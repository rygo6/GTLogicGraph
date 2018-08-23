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

            Debug.Log(_graph.GetInstanceID());
//            foreach (var node in graph.GetNodes<INode>())
//                AddNode(node);
//
//            foreach (var edge in graph.edges)
//                AddEdge(edge);

            Add(content);
        }

        GraphViewChange GraphViewChanged(GraphViewChange graphViewChange)
        {
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

        public void AddNode(NodeEditor node)
        {
            _graph.RegisterCompleteObjectUndo("Add Node " + node.DisplayName());
            _graph.AddNode(node.TargetLogicNode);
            EditorUtility.SetDirty(_graph);
            node.Owner = _graphView;
            var nodeView = new GenericNodeView {userData = node};
            _graphView.AddElement(nodeView);
            nodeView.Initialize(node, _edgeConnectorListener);
            nodeView.Dirty(ChangeType.Repaint);

//            if (m_SearchWindowProvider.nodeNeedsRepositioning &&
//                m_SearchWindowProvider.targetSlotReference.nodeGuid.Equals(node.guid))
//            {
//                m_SearchWindowProvider.nodeNeedsRepositioning = false;
//                foreach (var element in nodeView.inputContainer.Union(nodeView.outputContainer))
//                {
//                    var port = element as GenericPort;
//                    if (port == null)
//                        continue;
//                    if (port.slot.slotReference.Equals(m_SearchWindowProvider.targetSlotReference))
//                    {
//                        port.RegisterCallback<GeometryChangedEvent>(RepositionNode);
//                        return;
//                    }
//                }
//            }
        }

        public void AddEdge(Edge edge)
        {
            var leftSlot = (edge.output as GenericPort).slot;
            var rightSlot = (edge.input as GenericPort).slot;
            if (leftSlot == null || rightSlot == null)
            {
                Debug.Log("an edge is null");
                return;
            }

            _graph.RegisterCompleteObjectUndo("Connect Edge");
            GraphEdge graphEdge = new GraphEdge
            {
                Source = leftSlot.Owner.TargetLogicNode,
                SourceIndex = leftSlot.id,
                Target = rightSlot.Owner.TargetLogicNode,
                TargetIndex = rightSlot.id
            };
           
            _graph.AddEdge(graphEdge);
            
//            var sourceNode = m_Graph.GetNodeFromGuid(edge.outputSlot.nodeGuid);
//            if (sourceNode == null)
//            {
//                Debug.LogWarning("Source node is null");
//                return null;
//            }
//
//            var sourceSlot = sourceNode.FindOutputSlot<GenericSlot>(edge.outputSlot.slotId);
//
//            var targetNode = m_Graph.GetNodeFromGuid(edge.inputSlot.nodeGuid);
//            if (targetNode == null)
//            {
//                Debug.LogWarning("Target node is null");
//                return null;
//            }
//
//            var targetSlot = targetNode.FindInputSlot<GenericSlot>(edge.inputSlot.slotId);


            
            GenericNodeView sourceNodeView = _graphView.nodes.ToList().OfType<GenericNodeView>()
                .FirstOrDefault(x => x.NodeEditor == leftSlot.Owner);
            if (sourceNodeView != null)
            {
                GenericPort sourceAnchor = sourceNodeView.outputContainer.Children().OfType<GenericPort>()
                    .FirstOrDefault(x => x.slot.Equals(leftSlot));

                GenericNodeView targetNodeView = _graphView.nodes.ToList().OfType<GenericNodeView>()
                    .FirstOrDefault(x => x.NodeEditor == rightSlot.Owner);
                GenericPort targetAnchor = targetNodeView.inputContainer.Children().OfType<GenericPort>()
                    .FirstOrDefault(x => x.slot.Equals(rightSlot));

                var edgeView = new Edge
                {
                    userData = edge,
                    output = sourceAnchor,
                    input = targetAnchor
                };
                edgeView.output.Connect(edgeView);
                edgeView.input.Connect(edgeView);
                _graphView.AddElement(edgeView);
                sourceNodeView.RefreshPorts();
                targetNodeView.RefreshPorts();
//                sourceNodeView.UpdatePortInputTypes();
//                targetNodeView.UpdatePortInputTypes();
            }
        }
    }
}