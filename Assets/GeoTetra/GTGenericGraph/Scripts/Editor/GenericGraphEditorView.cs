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
        GenericGraph _graph;
        GenericGraphView _graphView;
        EditorWindow _editorWindow;
        GenericEdgeConnectorListener _edgeConnectorListener;
        GenericSearchWindowProvider _searchWindowProvider;

        public Action saveRequested { get; set; }

        public Action showInProjectRequested { get; set; }

        public GenericGraphView GenericGraphView
        {
            get { return _graphView; }
        }

        public GenericGraphEditorView(EditorWindow editorWindow, GenericGraph graph, string assetName)
        {
            Debug.Log(graph.GetInstanceID());
            _editorWindow = editorWindow;
            _graph = graph;

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
                _graphView = new GenericGraphView(_graph)
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

            _edgeConnectorListener = new GenericEdgeConnectorListener(this, _graph, _searchWindowProvider);

            _graphView.nodeCreationRequest = (c) =>
            {
                _searchWindowProvider.connectedPort = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), _searchWindowProvider);
            };

            for (int i = 0; i < graph.SerializedLogicNodes.Count; ++i)
            {
                AddNodeFromload(graph.SerializedLogicNodes[i]);
            }

            for (int i = 0; i < graph.SerializedEdges.Count; ++i)
            {
                AddEdgeFromLoad(graph.SerializedEdges[i]);
            }

            Add(content);
        }

        GraphViewChange GraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.edgesToCreate != null)
                Debug.Log(graphViewChange.edgesToCreate.Count);
            if (graphViewChange.elementsToRemove != null)
                Debug.Log(graphViewChange.elementsToRemove.Count);
            if (graphViewChange.movedElements != null)
                Debug.Log(graphViewChange.movedElements.Count);
            
//            if (graphViewChange.edgesToCreate != null)
//            {
//                foreach (var edge in graphViewChange.edgesToCreate)
//                {
//                    var leftSlot = edge.output.GetSlot();
//                    var rightSlot = edge.input.GetSlot();
//                    if (leftSlot != null && rightSlot != null)
//                    {
//                        m_Graph.owner.RegisterCompleteObjectUndo("Connect Edge");
//                        m_Graph.Connect(leftSlot.slotReference, rightSlot.slotReference);
//                    }
//                }
//                graphViewChange.edgesToCreate.Clear();
//            }
//
//            if (graphViewChange.movedElements != null)
//            {
//                foreach (var element in graphViewChange.movedElements)
//                {
//                    var node = element.userData as INode;
//                    if (node == null)
//                        continue;
//
//                    var drawState = node.drawState;
//                    drawState.position = element.GetPosition();
//                    node.drawState = drawState;
//                }
//            }
//
//            var nodesToUpdate = m_NodeViewHashSet;
//            nodesToUpdate.Clear();
//
//            if (graphViewChange.elementsToRemove != null)
//            {
//                m_Graph.owner.RegisterCompleteObjectUndo("Remove Elements");
//                m_Graph.RemoveElements(graphViewChange.elementsToRemove.OfType<GenericNodeView>().Select(v => (INode) v.node),
//                    graphViewChange.elementsToRemove.OfType<Edge>().Select(e => (IEdge) e.userData));
//                foreach (var edge in graphViewChange.elementsToRemove.OfType<Edge>())
//                {
//                    if (edge.input != null)
//                    {
//                        var materialNodeView = edge.input.node as GenericNodeView;
//                        if (materialNodeView != null)
//                            nodesToUpdate.Add(materialNodeView);
//                    }
//                    if (edge.output != null)
//                    {
//                        var materialNodeView = edge.output.node as GenericNodeView;
//                        if (materialNodeView != null)
//                            nodesToUpdate.Add(materialNodeView);
//                    }
//                }
//            }
//
//            foreach (var node in nodesToUpdate)
//                node.UpdatePortInputVisibilities();
//
//            UpdateEdgeColors(nodesToUpdate);

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

        public void AddNode(NodeEditor nodeEditor)
        {
            Undo.RegisterCompleteObjectUndo(_graph, "Add Node " + nodeEditor.NodeType());

            SerializedLogicNode serializedLogicNode = new SerializedLogicNode
            {
                NodeType = nodeEditor.NodeType(),
                JSON = JsonUtility.ToJson(nodeEditor)
            };

            _graph.AddNode(serializedLogicNode);
            EditorUtility.SetDirty(_graph);

            nodeEditor.Owner = _graphView;
            var nodeView = new GenericNodeView {userData = nodeEditor};
            _graphView.AddElement(nodeView);
            nodeView.Initialize(nodeEditor, _edgeConnectorListener);
            nodeView.Dirty(ChangeType.Repaint);
        }

        private void AddNodeFromload(SerializedLogicNode serializedLogicNode)
        {
            NodeEditor nodeEditor = null;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(NodeEditor)))
                    {
                        var attrs = type.GetCustomAttributes(typeof(NodeType), false) as NodeType[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            if (attrs[0].Name == serializedLogicNode.NodeType)
                            {
                                nodeEditor = (NodeEditor) Activator.CreateInstance(type);
                            }
                        }
                    }
                }
            }

            if (nodeEditor != null)
            {
                JsonUtility.FromJsonOverwrite(serializedLogicNode.JSON, nodeEditor);
                var nodeView = new GenericNodeView {userData = nodeEditor};
                _graphView.AddElement(nodeView);
                nodeView.Initialize(nodeEditor, _edgeConnectorListener);
                nodeView.Dirty(ChangeType.Repaint);
            }
            else
            {
                Debug.LogWarning("No NodeEditor found for " + serializedLogicNode);
            }
        }

        public void AddEdge(Edge edgeView)
        {
            GenericSlot leftSlot;
            GenericSlot rightSlot;
            GetSlots(edgeView, out leftSlot, out rightSlot);

            _graph.RegisterCompleteObjectUndo("Connect Edge");
            SerializedEdge serializedEdge = new SerializedEdge
            {
                Source = leftSlot.Owner.NodeGuid,
                SourceIndex = leftSlot.id,
                Target = rightSlot.Owner.NodeGuid,
                TargetIndex = rightSlot.id
            };

            _graph.AddEdge(serializedEdge);
            EditorUtility.SetDirty(_graph);

            edgeView.userData = serializedEdge;
            edgeView.output.Connect(edgeView);
            edgeView.input.Connect(edgeView);
            _graphView.AddElement(edgeView);
        }

        private void AddEdgeFromLoad(SerializedEdge serializedEdge)
        {
            GenericNodeView sourceNodeView = _graphView.nodes.ToList().OfType<GenericNodeView>()
                .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.Source);
            if (sourceNodeView != null)
            {
                GenericPort sourceAnchor = sourceNodeView.outputContainer.Children().OfType<GenericPort>()
                    .FirstOrDefault(x => x.slot.id == serializedEdge.SourceIndex);

                GenericNodeView targetNodeView = _graphView.nodes.ToList().OfType<GenericNodeView>()
                    .FirstOrDefault(x => x.NodeEditor.NodeGuid == serializedEdge.Target);
                GenericPort targetAnchor = targetNodeView.inputContainer.Children().OfType<GenericPort>()
                    .FirstOrDefault(x => x.slot.id == serializedEdge.TargetIndex);

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


        private void GetSlots(Edge edge, out GenericSlot leftSlot, out GenericSlot rightSlot)
        {
            leftSlot = (edge.output as GenericPort).slot;
            rightSlot = (edge.input as GenericPort).slot;
            if (leftSlot == null || rightSlot == null)
            {
                Debug.Log("an edge is null");
            }
        }
    }
}