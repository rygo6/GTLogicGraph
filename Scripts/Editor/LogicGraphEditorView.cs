using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Edge = UnityEditor.Experimental.UIElements.GraphView.Edge;

namespace GeoTetra.GTLogicGraph
{
    public class LogicGraphEditorView : VisualElement
    {
        private LogicGraphEditorObject _logicGraphEditorObject;
        private LogicGraphView _graphView;
        private EditorWindow _editorWindow;
        private EdgeConnectorListener _edgeConnectorListener;
        private SearchWindowProvider _searchWindowProvider;
        private bool _reloadGraph;

        public Action saveRequested { get; set; }

        public Action showInProjectRequested { get; set; }

        public LogicGraphView LogicGraphView
        {
            get { return _graphView; }
        }

        public LogicGraphEditorView(EditorWindow editorWindow, LogicGraphEditorObject logicGraphEditorObject)
        {
            Debug.Log(logicGraphEditorObject.GetInstanceID());
            _editorWindow = editorWindow;
            _logicGraphEditorObject = logicGraphEditorObject;
            _logicGraphEditorObject.Deserialized += LogicGraphEditorDataOnDeserialized;

            AddStyleSheetPath("Styles/LogicGraphEditorView");

            var toolbar = new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("Save Asset", EditorStyles.toolbarButton))
                {
                    if (saveRequested != null)
                        saveRequested();
                }

                GUILayout.Space(6);
                if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton))
                {
                    if (showInProjectRequested != null)
                        showInProjectRequested();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            });
            Add(toolbar);

            var content = new VisualElement {name = "content"};
            {
                _graphView = new LogicGraphView(_logicGraphEditorObject)
                {
                    name = "GraphView",
                    persistenceKey = "LogicGraphView"
                };

                _graphView.SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
                _graphView.AddManipulator(new ContentDragger());
                _graphView.AddManipulator(new SelectionDragger());
                _graphView.AddManipulator(new RectangleSelector());
                _graphView.AddManipulator(new ClickSelector());
                _graphView.RegisterCallback<KeyDownEvent>(KeyDown);
                content.Add(_graphView);

                _graphView.graphViewChanged = GraphViewChanged;
            }

            _searchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
            _searchWindowProvider.Initialize(editorWindow, this, _graphView);

            _edgeConnectorListener = new EdgeConnectorListener(this, _searchWindowProvider);

            _graphView.nodeCreationRequest = (c) =>
            {
                _searchWindowProvider.ConnectedPortView = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), _searchWindowProvider);
            };

            LoadElements();

            Add(content);
        }

        private void LoadElements()
        {
            for (int i = 0; i < _logicGraphEditorObject.LogicGraphData.SerializedNodes.Count; ++i)
            {
                AddNodeFromload(_logicGraphEditorObject.LogicGraphData.SerializedNodes[i]);
            }

            for (int i = 0; i < _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Count; ++i)
            {
                AddNodeFromload(_logicGraphEditorObject.LogicGraphData.SerializedInputNodes[i]);
            }

            for (int i = 0; i < _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Count; ++i)
            {
                AddNodeFromload(_logicGraphEditorObject.LogicGraphData.SerializedOutputNodes[i]);
            }

            for (int i = 0; i < _logicGraphEditorObject.LogicGraphData.SerializedEdges.Count; ++i)
            {
                AddEdgeFromLoad(_logicGraphEditorObject.LogicGraphData.SerializedEdges[i]);
            }
        }

        public void HandleGraphChanges()
        {
            if (_reloadGraph)
            {
                _reloadGraph = false;

                foreach (var nodeView in _graphView.nodes.ToList())
                {
                    Debug.Log("removing node " + nodeView);
                    _graphView.RemoveElement(nodeView);
                }

                foreach (var edge in _graphView.edges.ToList())
                {
                    Debug.Log("removing edge " + edge);
                    _graphView.RemoveElement(edge);
                }

                LoadElements();
            }
        }

        private void LogicGraphEditorDataOnDeserialized()
        {
            Debug.Log("GraphOnDeserialized");
            //comes after GraphData was undone, so reload graph
            _reloadGraph = true;
        }

        private GraphViewChange GraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.edgesToCreate != null)
                Debug.Log("EDGES TO CREATE " + graphViewChange.edgesToCreate.Count);

            if (graphViewChange.movedElements != null)
            {
                _logicGraphEditorObject.RegisterCompleteObjectUndo("Graph Element Moved.");
                foreach (var element in graphViewChange.movedElements)
                {
                    NodeEditor nodeEditor = element.userData as NodeEditor;
                    nodeEditor.Position = element.GetPosition().position;
                    nodeEditor.SerializedNode.JSON = JsonUtility.ToJson(nodeEditor);
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                _logicGraphEditorObject.RegisterCompleteObjectUndo("Deleted Graph Elements.");

                foreach (var nodeView in graphViewChange.elementsToRemove.OfType<LogicNodeView>())
                {
                    _logicGraphEditorObject.LogicGraphData.SerializedNodes.Remove(nodeView.NodeEditor.SerializedNode);
                    _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Remove(nodeView.NodeEditor
                        .SerializedNode);
                    _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Remove(nodeView.NodeEditor
                        .SerializedNode);
                }

                foreach (var edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    _logicGraphEditorObject.LogicGraphData.SerializedEdges.Remove(edge.userData as SerializedEdge);
                }
            }

            return graphViewChange;
        }


        private void KeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.Space && !evt.shiftKey && !evt.altKey && !evt.ctrlKey && !evt.commandKey)
            {
            }
            else if (evt.keyCode == KeyCode.F1)
            {
            }
        }

        public void AddNode(NodeEditor nodeEditor)
        {
            _logicGraphEditorObject.RegisterCompleteObjectUndo("Add Node " + nodeEditor.NodeType());

            SerializedNode serializedNode = new SerializedNode
            {
                NodeType = nodeEditor.NodeType(),
                JSON = JsonUtility.ToJson(nodeEditor)
            };

            nodeEditor.SerializedNode = serializedNode;
            if (nodeEditor is IInputNode)
            {
                _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Add(serializedNode);
            }
            else if (nodeEditor is IOutputNode)
            {
                _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Add(serializedNode);
            }
            else
            {
                _logicGraphEditorObject.LogicGraphData.SerializedNodes.Add(serializedNode);
            }

            nodeEditor.Owner = _graphView;
            var nodeView = new LogicNodeView {userData = nodeEditor};
            _graphView.AddElement(nodeView);
            nodeView.Initialize(nodeEditor, _edgeConnectorListener);
            nodeView.Dirty(ChangeType.Repaint);
        }

        private void AddNodeFromload(SerializedNode serializedNode)
        {
            NodeEditor nodeEditor = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeEditor)))
                    {
                        var attrs = type.GetCustomAttributes(typeof(NodeEditorType), false) as NodeEditorType[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            if (attrs[0].NodeType.Name == serializedNode.NodeType)
                            {
                                nodeEditor = (NodeEditor) Activator.CreateInstance(type);
                            }
                        }
                    }
                }
            }

            if (nodeEditor != null)
            {
                JsonUtility.FromJsonOverwrite(serializedNode.JSON, nodeEditor);
                nodeEditor.SerializedNode = serializedNode;
                nodeEditor.Owner = _graphView;
                var nodeView = new LogicNodeView {userData = nodeEditor};
                _graphView.AddElement(nodeView);
                nodeView.Initialize(nodeEditor, _edgeConnectorListener);
                nodeView.Dirty(ChangeType.Repaint);
            }
            else
            {
                Debug.LogWarning("No NodeEditor found for " + serializedNode);
            }
        }

        public void AddEdge(Edge edgeView)
        {
            PortDescription leftPortDescription;
            PortDescription rightPortDescription;
            GetSlots(edgeView, out leftPortDescription, out rightPortDescription);

            _logicGraphEditorObject.RegisterCompleteObjectUndo("Connect Edge");
            SerializedEdge serializedEdge = new SerializedEdge
            {
                SourceNodeGuid = leftPortDescription.Owner.NodeGuid,
                SourceMemberName = leftPortDescription.MemberName,
                TargetNodeGuid = rightPortDescription.Owner.NodeGuid,
                TargetMemberName = rightPortDescription.MemberName
            };

            _logicGraphEditorObject.LogicGraphData.SerializedEdges.Add(serializedEdge);

            edgeView.userData = serializedEdge;
            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);
            _graphView.AddElement(edgeView);
        }

        private void AddEdgeFromLoad(SerializedEdge serializedEdge)
        {
            LogicNodeView sourceNodeView = _graphView.nodes.ToList().OfType<LogicNodeView>()
                .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.SourceNodeGuid);
            if (sourceNodeView == null)
            {
                Debug.LogWarning($"Source NodeGUID not found {serializedEdge.SourceNodeGuid}");
                return;
            }

            PortView sourceAnchor = sourceNodeView.outputContainer.Children().OfType<PortView>()
                .FirstOrDefault(x => x.PortDescription.MemberName == serializedEdge.SourceMemberName);
            if (sourceAnchor == null)
            {
                Debug.LogError($"Source anchor null {serializedEdge.SourceMemberName}");
                return;
            }

            LogicNodeView targetNodeView = _graphView.nodes.ToList().OfType<LogicNodeView>()
                .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.TargetNodeGuid);
            if (targetNodeView == null)
            {
                Debug.LogWarning($"Target NodeGUID not found {serializedEdge.TargetNodeGuid}");
                return;
            }

            PortView targetAnchor = targetNodeView.inputContainer.Children().OfType<PortView>()
                .FirstOrDefault(x => x.PortDescription.MemberName == serializedEdge.TargetMemberName);

            var edgeView = new Edge
            {
                userData = serializedEdge,
                output = sourceAnchor,
                input = targetAnchor
            };
            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);
            _graphView.AddElement(edgeView);
        }


        private void GetSlots(Edge edge, out PortDescription leftPortDescription,
            out PortDescription rightPortDescription)
        {
            leftPortDescription = (edge.output as PortView).PortDescription;
            rightPortDescription = (edge.input as PortView).PortDescription;
            if (leftPortDescription == null || rightPortDescription == null)
            {
                Debug.Log("an edge is null");
            }
        }
    }
}