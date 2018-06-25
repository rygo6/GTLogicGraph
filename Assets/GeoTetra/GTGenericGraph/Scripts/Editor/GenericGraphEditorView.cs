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
        private AbstractGenericGraph _genericGraph;
        private GenericGraphView _genericGraphView;
        private EditorWindow _editorWindow;
        
        public GenericGraphView GenericGraphView
        {
            get { return _genericGraphView; }
        }
        
        public GenericGraphEditorView(EditorWindow editorWindow, string assetName)
        {
            _editorWindow = editorWindow;
            
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

            
            var content = new VisualElement { name = "content" };
            {
                _genericGraphView = new GenericGraphView() {name = "GraphView", persistenceKey = "GenericGraphView"};

                _genericGraphView.SetupZoom(0.05f, ContentZoomer.DefaultMaxScale);
                _genericGraphView.AddManipulator(new ContentDragger());
                _genericGraphView.AddManipulator(new SelectionDragger());
                _genericGraphView.AddManipulator(new RectangleSelector());
                _genericGraphView.AddManipulator(new ClickSelector());
//            _builderGraph.AddManipulator(new GraphDropTarget(graph));
                _genericGraphView.RegisterCallback<KeyDownEvent>(OnSpaceDown);
                content.Add(_genericGraphView);

                _genericGraphView.graphViewChanged = GraphViewChanged;
                
                RegisterCallback<PostLayoutEvent>(OnPostLayout);
            }
            
            _genericGraphView.nodeCreationRequest = (c) =>
            {
                Debug.Log("Open Search Window");
                AddNode(c.screenMousePosition);
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
            if(evt.keyCode == KeyCode.Space && !evt.shiftKey && !evt.altKey && !evt.ctrlKey && !evt.commandKey)
            {

            }
            else if (evt.keyCode == KeyCode.F1)
            {

            }
        }

        private void AddNode(Vector2 sceenMousePosition)
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
            var windowMousePosition = _editorWindow.GetRootVisualContainer().ChangeCoordinatesTo(_editorWindow.GetRootVisualContainer().parent, sceenMousePosition - _editorWindow.position.position);
            var graphMousePosition = _genericGraphView.contentViewContainer.WorldToLocal(windowMousePosition);
            drawState.position = new Rect(graphMousePosition, Vector2.zero);
            node.DrawState = drawState;
            
            var nodeView = new GenericNodeView();
            nodeView.Initialize(node);
            _genericGraphView.AddElement(nodeView);
        }
        
        Edge AddEdge(IEdge edge)
        {
            var sourceNode = _genericGraph.GetNodeFromGuid(edge.outputSlot.nodeGuid);
            if (sourceNode == null)
            {
                Debug.LogWarning("Source node is null");
                return null;
            }
            var sourceSlot = sourceNode.FindOutputSlot<MaterialSlot>(edge.outputSlot.slotId);

            var targetNode = _genericGraph.GetNodeFromGuid(edge.inputSlot.nodeGuid);
            if (targetNode == null)
            {
                Debug.LogWarning("Target node is null");
                return null;
            }
            var targetSlot = targetNode.FindInputSlot<MaterialSlot>(edge.inputSlot.slotId);

            var sourceNodeView = _genericGraphView.nodes.ToList().OfType<GenericNodeView>().FirstOrDefault(x => x.Node == sourceNode);
            if (sourceNodeView != null)
            {
                var sourceAnchor = sourceNodeView.outputContainer.Children().OfType<GenericPort>().FirstOrDefault(x => x.slot.Equals(sourceSlot));

                var targetNodeView = _genericGraphView.nodes.ToList().OfType<GenericNodeView>().FirstOrDefault(x => x.Node == targetNode);
                var targetAnchor = targetNodeView.inputContainer.Children().OfType<GenericPort>().FirstOrDefault(x => x.slot.Equals(targetSlot));

                var edgeView = new Edge
                {
                    userData = edge,
                    output = sourceAnchor,
                    input = targetAnchor
                };
                edgeView.output.Connect(edgeView);
                edgeView.input.Connect(edgeView);
                _genericGraphView.AddElement(edgeView);
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