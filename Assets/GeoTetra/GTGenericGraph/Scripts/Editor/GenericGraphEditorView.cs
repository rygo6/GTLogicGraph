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
        AbstractGenericGraph m_Graph;
        GenericGraphView m_GraphView;
        EditorWindow m_EditorWindow;
        GenericEdgeConnectorListener m_EdgeConnectorListener;
        GenericSearchWindowProvider m_SearchWindowProvider;

        public GenericGraphView GenericGraphView
        {
            get { return m_GraphView; }
        }

        public GenericGraphEditorView(EditorWindow editorWindow, AbstractGenericGraph graph, string assetName)
        {
            m_EditorWindow = editorWindow;
            m_Graph = graph;

            AddStyleSheetPath("Styles/GenericGraphEditorView");

            var toolbar = new IMGUIContainer(() =>
            {
                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("Save Asset", EditorStyles.toolbarButton))
                {
                }

                GUILayout.Space(6);
                if (GUILayout.Button("Show In Project", EditorStyles.toolbarButton))
                {
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            });
            Add(toolbar);


            var content = new VisualElement {name = "content"};
            {
                m_GraphView = new GenericGraphView(m_Graph) {name = "GraphView", persistenceKey = "GenericGraphView"};

                m_GraphView.SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
                m_GraphView.AddManipulator(new ContentDragger());
                m_GraphView.AddManipulator(new SelectionDragger());
                m_GraphView.AddManipulator(new RectangleSelector());
                m_GraphView.AddManipulator(new ClickSelector());
//                m_GraphView.AddManipulator(new GraphDropTarget(m_Graph));
                m_GraphView.RegisterCallback<KeyDownEvent>(OnSpaceDown);
                content.Add(m_GraphView);

                m_GraphView.graphViewChanged = GraphViewChanged;

                RegisterCallback<PostLayoutEvent>(OnPostLayout);
            }

            m_SearchWindowProvider = ScriptableObject.CreateInstance<GenericSearchWindowProvider>();
            m_SearchWindowProvider.Initialize(editorWindow, m_Graph, m_GraphView);
            m_EdgeConnectorListener = new GenericEdgeConnectorListener(m_Graph, m_SearchWindowProvider);

            m_GraphView.nodeCreationRequest = (c) =>
            {
                Debug.Log("Open Search Window");
//                AddNewNode(c.screenMousePosition);
                m_SearchWindowProvider.connectedPort = null;
                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), m_SearchWindowProvider);
            };

            foreach (var node in graph.GetNodes<INode>())
                AddNode(node);

            foreach (var edge in graph.edges)
                AddEdge(edge);

            Add(content);
        }

        private GraphViewChange GraphViewChanged(GraphViewChange graphViewChange)
        {
            return graphViewChange;
        }

        private void OnPostLayout(PostLayoutEvent evt)
        {
//            Debug.Log("OnPostLayout BuilderGraphView" + evt.newRect);
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

        HashSet<GenericNodeView> m_NodeViewHashSet = new HashSet<GenericNodeView>();

        public void HandleGraphChanges()
        {
//            m_BlackboardProvider.HandleGraphChanges();

            foreach (var node in m_Graph.removedNodes)
            {
                node.UnregisterCallback(OnNodeChanged);
                var nodeView = m_GraphView.nodes.ToList().OfType<MaterialNodeView>()
                    .FirstOrDefault(p => p.node != null && p.node.guid == node.guid);
                if (nodeView != null)
                {
                    nodeView.Dispose();
                    nodeView.userData = null;
                    m_GraphView.RemoveElement(nodeView);
                }
            }

            foreach (var node in m_Graph.addedNodes)
            {
                AddNode(node);
            }

            foreach (var node in m_Graph.pastedNodes)
            {
                var nodeView = m_GraphView.nodes.ToList().OfType<MaterialNodeView>()
                    .FirstOrDefault(p => p.node != null && p.node.guid == node.guid);
                m_GraphView.AddToSelection(nodeView);
            }

            var nodesToUpdate = m_NodeViewHashSet;
            nodesToUpdate.Clear();

            foreach (var edge in m_Graph.removedEdges)
            {
                var edgeView = m_GraphView.graphElements.ToList().OfType<Edge>()
                    .FirstOrDefault(p => p.userData is IEdge && Equals((IEdge) p.userData, edge));
                if (edgeView != null)
                {
                    var nodeView = edgeView.input.node as GenericNodeView;
                    if (nodeView != null && nodeView.node != null)
                    {
                        nodesToUpdate.Add(nodeView);
                    }

                    edgeView.output.Disconnect(edgeView);
                    edgeView.input.Disconnect(edgeView);

                    edgeView.output = null;
                    edgeView.input = null;

                    m_GraphView.RemoveElement(edgeView);
                }
            }

            foreach (var edge in m_Graph.addedEdges)
            {
                var edgeView = AddEdge(edge);
                if (edgeView != null)
                    nodesToUpdate.Add((GenericNodeView) edgeView.input.node);
            }

            foreach (var node in nodesToUpdate)
                node.UpdatePortInputVisibilities();

            UpdateEdgeColors(nodesToUpdate);
        }

        Stack<GenericNodeView> m_NodeStack = new Stack<GenericNodeView>();

        void UpdateEdgeColors(HashSet<GenericNodeView> nodeViews)
        {
            var nodeStack = m_NodeStack;
            nodeStack.Clear();
            foreach (var nodeView in nodeViews)
                nodeStack.Push(nodeView);
            while (nodeStack.Any())
            {
                var nodeView = nodeStack.Pop();
                nodeView.UpdatePortInputTypes();
                foreach (var anchorView in nodeView.outputContainer.Children().OfType<Port>())
                {
                    foreach (var edgeView in anchorView.connections.OfType<Edge>())
                    {
                        var targetSlot = edgeView.input.GetSlot();
                        if (targetSlot.valueType == SlotValueType.DynamicVector ||
                            targetSlot.valueType == SlotValueType.DynamicMatrix ||
                            targetSlot.valueType == SlotValueType.Dynamic)
                        {
                            var connectedNodeView = edgeView.input.node as GenericNodeView;
                            if (connectedNodeView != null && !nodeViews.Contains(connectedNodeView))
                            {
                                nodeStack.Push(connectedNodeView);
                                nodeViews.Add(connectedNodeView);
                            }
                        }
                    }
                }

                foreach (var anchorView in nodeView.inputContainer.Children().OfType<Port>())
                {
                    var targetSlot = anchorView.GetSlot();
                    if (targetSlot.valueType != SlotValueType.DynamicVector)
                        continue;
                    foreach (var edgeView in anchorView.connections.OfType<Edge>())
                    {
                        var connectedNodeView = edgeView.output.node as GenericNodeView;
                        if (connectedNodeView != null && !nodeViews.Contains(connectedNodeView))
                        {
                            nodeStack.Push(connectedNodeView);
                            nodeViews.Add(connectedNodeView);
                        }
                    }
                }
            }
        }

        void OnNodeChanged(INode inNode, ModificationScope scope)
        {
            if (m_GraphView == null)
                return;

            var dependentNodes = new List<INode>();
            NodeUtils.CollectNodesNodeFeedsInto(dependentNodes, inNode);
            foreach (var node in dependentNodes)
            {
                var theViews = m_GraphView.nodes.ToList().OfType<GenericNodeView>();
                var viewsFound = theViews.Where(x => x.node.guid == node.guid).ToList();
                foreach (var drawableNodeData in viewsFound)
                    drawableNodeData.OnModified(scope);
            }
        }

        void AddNode(INode node)
        {
            var nodeView = new GenericNodeView {userData = node};
            m_GraphView.AddElement(nodeView);
            nodeView.Initialize(node as AbstractGenericNode, m_EdgeConnectorListener);
            node.RegisterCallback(OnNodeChanged);
            nodeView.Dirty(ChangeType.Repaint);

            if (m_SearchWindowProvider.nodeNeedsRepositioning &&
                m_SearchWindowProvider.targetSlotReference.nodeGuid.Equals(node.guid))
            {
                m_SearchWindowProvider.nodeNeedsRepositioning = false;
                foreach (var element in nodeView.inputContainer.Union(nodeView.outputContainer))
                {
                    var port = element as GenericPort;
                    if (port == null)
                        continue;
                    if (port.slot.slotReference.Equals(m_SearchWindowProvider.targetSlotReference))
                    {
                        port.RegisterCallback<PostLayoutEvent>(RepositionNode);
                        return;
                    }
                }
            }
        }

        static void RepositionNode(PostLayoutEvent evt)
        {
            var port = evt.target as GenericPort;
            if (port == null)
                return;
            port.UnregisterCallback<PostLayoutEvent>(RepositionNode);
            var nodeView = port.node as MaterialNodeView;
            if (nodeView == null)
                return;
            var offset = nodeView.mainContainer.WorldToLocal(port.GetGlobalCenter() + new Vector3(3f, 3f, 0f));
            var position = nodeView.GetPosition();
            position.position -= offset;
            nodeView.SetPosition(position);
            var drawState = nodeView.node.DrawState;
            drawState.position = position;
            nodeView.node.DrawState = drawState;
            nodeView.Dirty(ChangeType.Repaint);
            port.Dirty(ChangeType.Repaint);
        }

        Edge AddEdge(IEdge edge)
        {
            var sourceNode = m_Graph.GetNodeFromGuid(edge.outputSlot.nodeGuid);
            if (sourceNode == null)
            {
                Debug.LogWarning("Source node is null");
                return null;
            }

            var sourceSlot = sourceNode.FindOutputSlot<GenericSlot>(edge.outputSlot.slotId);

            var targetNode = m_Graph.GetNodeFromGuid(edge.inputSlot.nodeGuid);
            if (targetNode == null)
            {
                Debug.LogWarning("Target node is null");
                return null;
            }

            var targetSlot = targetNode.FindInputSlot<GenericSlot>(edge.inputSlot.slotId);

            Debug.Log(m_GraphView.nodes.ToList().OfType<GenericNodeView>().Count());
            
            var sourceNodeView = m_GraphView.nodes.ToList().OfType<GenericNodeView>()
                .FirstOrDefault(x => x.node == sourceNode);
            if (sourceNodeView != null)
            {
                var sourceAnchor = sourceNodeView.outputContainer.Children().OfType<GenericPort>()
                    .FirstOrDefault(x => x.slot.Equals(sourceSlot));

                var targetNodeView = m_GraphView.nodes.ToList().OfType<GenericNodeView>()
                    .FirstOrDefault(x => x.node == targetNode);
                var targetAnchor = targetNodeView.inputContainer.Children().OfType<GenericPort>()
                    .FirstOrDefault(x => x.slot.Equals(targetSlot));

                var edgeView = new Edge
                {
                    userData = edge,
                    output = sourceAnchor,
                    input = targetAnchor
                };
                edgeView.output.Connect(edgeView);
                edgeView.input.Connect(edgeView);
                m_GraphView.AddElement(edgeView);
                sourceNodeView.RefreshPorts();
                targetNodeView.RefreshPorts();
                sourceNodeView.UpdatePortInputTypes();
                targetNodeView.UpdatePortInputTypes();

                return edgeView;
            }

            return null;
        }
    }
}