using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.UI;
using Edge = UnityEditor.Experimental.UIElements.GraphView.Edge;

namespace GeoTetra.GTGenericGraph
{
    public class GenericGraphEditorView : VisualElement
    {
        private GraphData _graphData;
        private GenericGraphView _graphView;
        private EditorWindow _editorWindow;
        private GenericEdgeConnectorListener _edgeConnectorListener;
        private GenericSearchWindowProvider _searchWindowProvider;
        private bool _reloadGraph;

        public Action saveRequested { get; set; }

        public Action showInProjectRequested { get; set; }

        public GenericGraphView GenericGraphView
        {
            get { return _graphView; }
        }

        public GenericGraphEditorView(EditorWindow editorWindow, GraphData graphData, string assetName)
        {
            Debug.Log(graphData.GetInstanceID());
            _editorWindow = editorWindow;
            _graphData = graphData;
            _graphData.Deserialized += GraphDataOnDeserialized;

            AddStyleSheetPath("Styles/GenericGraphEditorView");

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
                _graphView = new GenericGraphView(_graphData)
                {
                    name = "GraphView",
                    persistenceKey = "GenericGraphView"
                };

                _graphView.SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
                _graphView.AddManipulator(new ContentDragger());
                _graphView.AddManipulator(new SelectionDragger());
                _graphView.AddManipulator(new RectangleSelector());
                _graphView.AddManipulator(new ClickSelector());
                _graphView.RegisterCallback<KeyDownEvent>(OnSpaceDown);
                content.Add(_graphView);

                _graphView.graphViewChanged = GraphViewChanged;
            }

            _searchWindowProvider = ScriptableObject.CreateInstance<GenericSearchWindowProvider>();
            _searchWindowProvider.Initialize(editorWindow, this, _graphView);

            _edgeConnectorListener = new GenericEdgeConnectorListener(this, _graphData, _searchWindowProvider);

            _graphView.nodeCreationRequest = (c) =>
            {
                _searchWindowProvider.connectedPort = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), _searchWindowProvider);
            };

            LoadElements();

            Add(content);
        }

        private void LoadElements()
        {
            for (int i = 0; i < _graphData.SerializedNodes.Count; ++i)
            {
                AddNodeFromload(_graphData.SerializedNodes[i]);
            }

            for (int i = 0; i < _graphData.SerializedEdges.Count; ++i)
            {
                AddEdgeFromLoad(_graphData.SerializedEdges[i]);
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
        
        private void GraphDataOnDeserialized()
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
                foreach (var element in graphViewChange.movedElements)
                {
                    NodeDescription nodeDescription = element.userData as NodeDescription;
                    nodeDescription.Position = element.GetPosition().position;
                    nodeDescription.SerializedNode.JSON = JsonUtility.ToJson(nodeDescription);
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                foreach (var nodeView in graphViewChange.elementsToRemove.OfType<GenericNodeView>())
                {
                    _graphData.RemoveNode(nodeView.NodeDescription.SerializedNode);
                }

                foreach (var edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    _graphData.RemoveEdge(edge.userData as SerializedEdge);
                }
            }

            return graphViewChange;
        }


        private void OnSpaceDown(KeyDownEvent evt)
        {
            Debug.Log(evt);
            if (evt.keyCode == KeyCode.Space && !evt.shiftKey && !evt.altKey && !evt.ctrlKey && !evt.commandKey)
            {
            }
            else if (evt.keyCode == KeyCode.F1)
            {
            }
        }

        public void AddNode(NodeDescription nodeDescription)
        {
            _graphData.RegisterCompleteObjectUndo("Add Node " + nodeDescription.NodeType());

            SerializedNode serializedNode = new SerializedNode
            {
                NodeType = nodeDescription.NodeType(),
                JSON = JsonUtility.ToJson(nodeDescription)
            };

            nodeDescription.SerializedNode = serializedNode;
            _graphData.AddNode(serializedNode);
            EditorUtility.SetDirty(_graphData);

            nodeDescription.Owner = _graphView;
            var nodeView = new GenericNodeView {userData = nodeDescription};
            _graphView.AddElement(nodeView);
            nodeView.Initialize(nodeDescription, _edgeConnectorListener);
            nodeView.Dirty(ChangeType.Repaint);
        }

        private void AddNodeFromload(SerializedNode serializedNode)
        {
            NodeDescription nodeDescription = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeDescription)))
                    {
                        var attrs = type.GetCustomAttributes(typeof(NodeType), false) as NodeType[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            if (attrs[0].Name == serializedNode.NodeType)
                            {
                                nodeDescription = (NodeDescription) Activator.CreateInstance(type);
                            }
                        }
                    }
                }
            }

            if (nodeDescription != null)
            {
                JsonUtility.FromJsonOverwrite(serializedNode.JSON, nodeDescription);
                nodeDescription.SerializedNode = serializedNode;
                nodeDescription.Owner = _graphView;
                var nodeView = new GenericNodeView {userData = nodeDescription};
                _graphView.AddElement(nodeView);
                nodeView.Initialize(nodeDescription, _edgeConnectorListener);
                nodeView.Dirty(ChangeType.Repaint);
            }
            else
            {
                Debug.LogWarning("No NodeEditor found for " + serializedNode);
            }
        }

        public void AddEdge(Edge edgeView)
        {
            GenericPortDescription leftPortDescription;
            GenericPortDescription rightPortDescription;
            GetSlots(edgeView, out leftPortDescription, out rightPortDescription);

            _graphData.RegisterCompleteObjectUndo("Connect Edge");
            SerializedEdge serializedEdge = new SerializedEdge
            {
                Source = leftPortDescription.Owner.NodeGuid,
                SourceIndex = leftPortDescription.id,
                Target = rightPortDescription.Owner.NodeGuid,
                TargetIndex = rightPortDescription.id
            };

            _graphData.AddEdge(serializedEdge);
            EditorUtility.SetDirty(_graphData);

            edgeView.userData = serializedEdge;
            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);
            _graphView.AddElement(edgeView);
        }

        private void AddEdgeFromLoad(SerializedEdge serializedEdge)
        {
            GenericNodeView sourceNodeView = _graphView.nodes.ToList().OfType<GenericNodeView>()
                .FirstOrDefault(x => x.NodeDescription.NodeGuid == serializedEdge.Source);
            if (sourceNodeView != null)
            {
                GenericPort sourceAnchor = sourceNodeView.outputContainer.Children().OfType<GenericPort>()
                    .FirstOrDefault(x => x.PortDescription.id == serializedEdge.SourceIndex);

                GenericNodeView targetNodeView = _graphView.nodes.ToList().OfType<GenericNodeView>()
                    .FirstOrDefault(x => x.NodeDescription.NodeGuid == serializedEdge.Target);
                GenericPort targetAnchor = targetNodeView.inputContainer.Children().OfType<GenericPort>()
                    .FirstOrDefault(x => x.PortDescription.id == serializedEdge.TargetIndex);

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
        }


        private void GetSlots(Edge edge, out GenericPortDescription leftPortDescription, out GenericPortDescription rightPortDescription)
        {
            leftPortDescription = (edge.output as GenericPort).PortDescription;
            rightPortDescription = (edge.input as GenericPort).PortDescription;
            if (leftPortDescription == null || rightPortDescription == null)
            {
                Debug.Log("an edge is null");
            }
        }
    }
}