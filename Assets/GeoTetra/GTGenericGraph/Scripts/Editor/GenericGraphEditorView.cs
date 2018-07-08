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

        public GenericGraphEditorView(EditorWindow editorWindow,  AbstractGenericGraph graph, string assetName)
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
            m_EdgeConnectorListener = new GenericEdgeConnectorListener(m_Graph, m_SearchWindowProvider );

            m_GraphView.nodeCreationRequest = (c) =>
            {
                Debug.Log("Open Search Window");
                AddNewNode(c.screenMousePosition);
//                m_SearchWindowProvider.connectedPort = null;
//                SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), m_SearchWindowProvider);
            };

            Add(content);
        }

        private GraphViewChange GraphViewChanged(GraphViewChange graphViewChange)
        {
            return graphViewChange;
        }

        private void OnPostLayout(PostLayoutEvent evt)
        {
            Debug.Log("OnPostLayout BuilderGraphView" + evt.newRect);
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

        private void AddNewNode(Vector2 sceenMousePosition)
        {
            Debug.Log("Adding Node");
            var node = new ExampleGenericNode();
            var slot = new Vector1GenericSlot(0, "testBool", "tesBoolIn", SlotType.Input, 1);
            node.AddSlot(slot);
            var slot2 = new Vector1GenericSlot(1, "testBool2", "tesBoolOut", SlotType.Output, 2);
            node.AddSlot(slot2);

            var slot3 = new Vector1GenericSlot(2, "testBool3", "tesBoolIn2", SlotType.Input, 3);
            node.AddSlot(slot3);
            var slot4 = new Vector1GenericSlot(3, "testBool4", "tesBoolOut2", SlotType.Output, 4);
            node.AddSlot(slot4);

            var drawState = node.DrawState;
            var windowMousePosition = m_EditorWindow.GetRootVisualContainer()
                .ChangeCoordinatesTo(m_EditorWindow.GetRootVisualContainer().parent,
                    sceenMousePosition - m_EditorWindow.position.position);
            var graphMousePosition = m_GraphView.contentViewContainer.WorldToLocal(windowMousePosition);
            drawState.position = new Rect(graphMousePosition, Vector2.zero);
            node.DrawState = drawState;

            m_Graph.owner.RegisterCompleteObjectUndo("Add " + node.name);
            m_Graph.AddNode(node);
            
//            if (connectedPort != null)
//            {
//                var connectedSlot = connectedPort.slot;
//                var connectedSlotReference = connectedSlot.owner.GetSlotReference(connectedSlot.id);
//                var compatibleSlotReference = node.GetSlotReference(nodeEntry.compatibleSlotId);
//
//                var fromReference = connectedSlot.isOutputSlot ? connectedSlotReference : compatibleSlotReference;
//                var toReference = connectedSlot.isOutputSlot ? compatibleSlotReference : connectedSlotReference;
//                m_Graph.Connect(fromReference, toReference);
//
//                nodeNeedsRepositioning = true;
//                targetSlotReference = compatibleSlotReference;
//                targetPosition = graphMousePosition;
//            }
            
            var nodeView = new GenericNodeView();
            nodeView.Initialize(node, m_EdgeConnectorListener);
            m_GraphView.AddElement(nodeView);
        }

        Edge AddEdge(IEdge edge)
        {
            var sourceNode = m_Graph.GetNodeFromGuid(edge.outputSlot.nodeGuid);
            if (sourceNode == null)
            {
                Debug.LogWarning("Source node is null");
                return null;
            }

            var sourceSlot = sourceNode.FindOutputSlot<MaterialSlot>(edge.outputSlot.slotId);

            var targetNode = m_Graph.GetNodeFromGuid(edge.inputSlot.nodeGuid);
            if (targetNode == null)
            {
                Debug.LogWarning("Target node is null");
                return null;
            }

            var targetSlot = targetNode.FindInputSlot<MaterialSlot>(edge.inputSlot.slotId);

            var sourceNodeView = m_GraphView.nodes.ToList().OfType<GenericNodeView>()
                .FirstOrDefault(x => x.node == sourceNode);
            if (sourceNodeView != null)
            {
                var sourceAnchor = sourceNodeView.outputContainer.Children().OfType<GenericPort>()
                    .FirstOrDefault(x => x.Slot.Equals(sourceSlot));

                var targetNodeView = m_GraphView.nodes.ToList().OfType<GenericNodeView>()
                    .FirstOrDefault(x => x.node == targetNode);
                var targetAnchor = targetNodeView.inputContainer.Children().OfType<GenericPort>()
                    .FirstOrDefault(x => x.Slot.Equals(targetSlot));

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