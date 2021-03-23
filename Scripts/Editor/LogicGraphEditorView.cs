﻿using System;
using System.Collections.Generic;
using System.Linq;
using GeoTetra.GTLogicGraph.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    /// <summary>
    /// Root Visual element which takes up entire editor window. Every other visual element is a child
    /// of this.
    /// </summary>
    public class LogicGraphEditorView : VisualElement
    {
        private LogicGraphEditorObject _logicGraphEditorObject;
        private LogicGraphView _graphView;
        private LogicGraphEditorWindow _editorWindow;
        private EdgeConnectorListener _edgeConnectorListener;
        private SearchWindowProvider _searchWindowProvider;
        private bool _reloadGraph;

        public Action saveRequested { get; set; }

        public Action showInProjectRequested { get; set; }

        public LogicGraphView LogicGraphView => _graphView;

        public LogicGraphEditorView(LogicGraphEditorWindow editorWindow, LogicGraphEditorObject logicGraphEditorObject)
        {
            Debug.Log(logicGraphEditorObject.GetInstanceID());
            _editorWindow = editorWindow;
            _logicGraphEditorObject = logicGraphEditorObject;
            _logicGraphEditorObject.Deserialized += LogicGraphEditorDataOnDeserialized;

             this.LoadAndAddStyleSheet("Styles/LogicGraphEditorView");

            var toolbar = new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("Save Asset", EditorStyles.toolbarButton))
                {
                    saveRequested?.Invoke();
                }

                GUILayout.Space(6);
                if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton))
                {
                    showInProjectRequested?.Invoke();
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
//                    persistenceKey = "LogicGraphView",
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
                _searchWindowProvider.ConnectedLogicPort = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), _searchWindowProvider);
            };

            LoadElements();

            Add(content);
        }

        private void LoadElements()
        {
            bool errorCorrected = false;
            
            for (int i = _logicGraphEditorObject.LogicGraphData.SerializedNodes.Count - 1; i > -1; --i)
            {
                AddNode(_logicGraphEditorObject.LogicGraphData.SerializedNodes[i]);
            }

            for (int i = _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Count - 1; i > -1; --i)
            {
                AddNode(_logicGraphEditorObject.LogicGraphData.SerializedInputNodes[i]);
            }

            for (int i = _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Count - 1; i > -1; --i)
            {
                AddNode(_logicGraphEditorObject.LogicGraphData.SerializedOutputNodes[i]);
            }

            for (int i = _logicGraphEditorObject.LogicGraphData.SerializedEdges.Count - 1; i > -1; --i)
            {
                if (!AddEdge(_logicGraphEditorObject.LogicGraphData.SerializedEdges[i]))
                {
                    Debug.Log("Removing erroneous edge.");
                    _logicGraphEditorObject.LogicGraphData.SerializedEdges.RemoveAt(i);
                    errorCorrected = true;
                }
            }

            foreach (var node in _graphView.nodes.ToList())
            {
                
            }
            

            if (errorCorrected)
            {
                saveRequested?.Invoke();
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
            Debug.Log($"GraphViewChanged {graphViewChange}");
            
            if (graphViewChange.edgesToCreate != null)
                Debug.Log("EDGES TO CREATE " + graphViewChange.edgesToCreate.Count);

            if (graphViewChange.movedElements != null)
            {
                Debug.Log("Moved elements " + graphViewChange.movedElements.Count);
                _logicGraphEditorObject.RegisterCompleteObjectUndo("Graph Element Moved.");
                foreach (var element in graphViewChange.movedElements)
                {
                    AbstractLogicNodeEditor logicNodeEditor = element.userData as AbstractLogicNodeEditor;
                    logicNodeEditor.Position = element.GetPosition().position;
                    logicNodeEditor.SerializedNode.JSON = JsonUtility.ToJson(logicNodeEditor);
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                Debug.Log("Elements to remove" + graphViewChange.elementsToRemove.Count);
                _logicGraphEditorObject.RegisterCompleteObjectUndo("Deleted Graph Elements.");

                foreach (var nodeView in graphViewChange.elementsToRemove.OfType<LogicNodeView>())
                {
                    _logicGraphEditorObject.LogicGraphData.SerializedNodes.Remove(nodeView.LogicNodeEditor.SerializedNode);
                    _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Remove(nodeView.LogicNodeEditor
                        .SerializedNode);
                    _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Remove(nodeView.LogicNodeEditor
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

        public SerializedNode AddNode(AbstractLogicNodeEditor logicNodeEditor)
        {
            _logicGraphEditorObject.RegisterCompleteObjectUndo("Add Node " + logicNodeEditor.NodeType());

            SerializedNode serializedNode = new SerializedNode
            {
                NodeType = logicNodeEditor.NodeType(),
                JSON = JsonUtility.ToJson(logicNodeEditor)
            };

            logicNodeEditor.SerializedNode = serializedNode;
            if (logicNodeEditor is IInputNode)
            {
                _logicGraphEditorObject.LogicGraphData.SerializedInputNodes.Add(serializedNode);
            }
            else if (logicNodeEditor is IOutputNode)
            {
                _logicGraphEditorObject.LogicGraphData.SerializedOutputNodes.Add(serializedNode);
            }
            else
            {
                _logicGraphEditorObject.LogicGraphData.SerializedNodes.Add(serializedNode);
            }

            logicNodeEditor.Owner = _graphView;
            var nodeView = new LogicNodeView {userData = logicNodeEditor};
            _graphView.AddElement(nodeView);
            nodeView.Initialize(logicNodeEditor, _edgeConnectorListener);
            nodeView.MarkDirtyRepaint();
            return serializedNode;
        }

        private void AddNode(SerializedNode serializedNode)
        {
            AbstractLogicNodeEditor logicNodeEditor = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(AbstractLogicNodeEditor)))
                    {
                        var attrs = type.GetCustomAttributes(typeof(NodeEditorType), false) as NodeEditorType[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            if (attrs[0].NodeType.Name == serializedNode.NodeType)
                            {
                                logicNodeEditor = (AbstractLogicNodeEditor) Activator.CreateInstance(type);
                            }
                        }
                    }
                }
            }

            if (logicNodeEditor != null)
            {
                JsonUtility.FromJsonOverwrite(serializedNode.JSON, logicNodeEditor);
                logicNodeEditor.SerializedNode = serializedNode;
                logicNodeEditor.Owner = _graphView;
                var nodeView = new LogicNodeView {userData = logicNodeEditor};
                _graphView.AddElement(nodeView);
                nodeView.Initialize(logicNodeEditor, _edgeConnectorListener);
                nodeView.MarkDirtyRepaint();
            }
            else
            {
                Debug.LogWarning("No NodeEditor found for " + serializedNode);
            }
        }

        public void RemoveEdgesConnectedTo(LogicPort logicPort)
        {
            _foundEdges.Clear();
            _logicGraphEditorObject.LogicGraphData.GetEdges(logicPort.Slot.Owner.NodeGuid, logicPort.Slot.MemberName, _foundEdges);
            for (int i = 0; i < _foundEdges.Count; ++i)
            {
                RemoveEdge(_foundEdges[i]);
            }
        }
       
        private readonly List<SerializedEdge> _foundEdges = new List<SerializedEdge>();

        public void RemoveEdge(SerializedEdge serializedEdge)
        {
            _logicGraphEditorObject.LogicGraphData.SerializedEdges.Remove(serializedEdge);
            foreach (var edge in _graphView.edges.ToList())
            {
                if (edge.userData == serializedEdge)
                {
                    Debug.Log("removing edge " + edge);
                    _graphView.RemoveElement(edge);
                }
            }
        }
        
        public void AddEdge(Edge edgeView)
        {
            LogicSlot leftLogicSlot;
            LogicSlot rightLogicSlot;
            GetSlots(edgeView, out leftLogicSlot, out rightLogicSlot);

            _logicGraphEditorObject.RegisterCompleteObjectUndo("Connect Edge");
            SerializedEdge serializedEdge = new SerializedEdge
            {
                SourceNodeGuid = leftLogicSlot.Owner.NodeGuid,
                SourceMemberName = leftLogicSlot.MemberName,
                TargetNodeGuid = rightLogicSlot.Owner.NodeGuid,
                TargetMemberName = rightLogicSlot.MemberName
            };

            _logicGraphEditorObject.LogicGraphData.SerializedEdges.Add(serializedEdge);

            edgeView.userData = serializedEdge;
            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);
            _graphView.AddElement(edgeView);
        }
        
        public void AddEdge(LogicSlot leftLogicSlot, LogicSlot rightLogicSlot )
        {
            SerializedEdge serializedEdge = new SerializedEdge
            {
                SourceNodeGuid = leftLogicSlot.Owner.NodeGuid,
                SourceMemberName = leftLogicSlot.MemberName,
                TargetNodeGuid = rightLogicSlot.Owner.NodeGuid,
                TargetMemberName = rightLogicSlot.MemberName
            };

            _logicGraphEditorObject.LogicGraphData.SerializedEdges.Add(serializedEdge);

            AddEdge(serializedEdge);
        }

        public bool AddEdge(SerializedEdge serializedEdge)
        {
            LogicNodeView sourceNodeView = _graphView.nodes.ToList().OfType<LogicNodeView>()
                .FirstOrDefault(x => x.LogicNodeEditor.NodeGuid == serializedEdge.SourceNodeGuid);
            if (sourceNodeView == null)
            {
                Debug.LogWarning($"Source NodeGUID not found {serializedEdge.SourceNodeGuid}");
                return false;
            }

            LogicPort sourceAnchor = sourceNodeView.outputContainer.Children().OfType<LogicPort>()
                .FirstOrDefault(x => x.Slot.MemberName == serializedEdge.SourceMemberName);
            if (sourceAnchor == null)
            {
                Debug.LogError($"Source anchor null {serializedEdge.SourceMemberName} {serializedEdge.SourceNodeGuid}");
                return false;
            }

            LogicNodeView targetNodeView = _graphView.nodes.ToList().OfType<LogicNodeView>()
                .FirstOrDefault(x => x.LogicNodeEditor.NodeGuid == serializedEdge.TargetNodeGuid);
            if (targetNodeView == null)
            {
                Debug.LogWarning($"Target NodeGUID not found {serializedEdge.TargetNodeGuid}");
                return false;
            }

            LogicPort targetAnchor = targetNodeView.inputContainer.Children().OfType<LogicPort>()
                .FirstOrDefault(x => x.Slot.MemberName == serializedEdge.TargetMemberName);
            if (targetAnchor == null)
            {
                Debug.LogError($"Target anchor null {serializedEdge.SourceMemberName} {serializedEdge.TargetNodeGuid}");
                return false;
            }

            var edgeView = new Edge
            {
                userData = serializedEdge,
                output = sourceAnchor,
                input = targetAnchor
            };
            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);
            _graphView.AddElement(edgeView);
            targetNodeView.UpdatePortInputVisibilities();
            sourceNodeView.UpdatePortInputVisibilities();

            return true;
        }


        private void GetSlots(Edge edge, out LogicSlot leftLogicSlot,
            out LogicSlot rightLogicSlot)
        {
            leftLogicSlot = (edge.output as LogicPort).Slot;
            rightLogicSlot = (edge.input as LogicPort).Slot;
            if (leftLogicSlot == null || rightLogicSlot == null)
            {
                Debug.Log("an edge is null");
            }
        }
    }
}