using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace GeoTetra.GTGenericGraph
{
    public class GenericGraphEditorView : VisualElement
    {
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
            
            var drawState = node.DrawState;
            var windowMousePosition = _editorWindow.GetRootVisualContainer().ChangeCoordinatesTo(_editorWindow.GetRootVisualContainer().parent, sceenMousePosition - _editorWindow.position.position);
            var graphMousePosition = _genericGraphView.contentViewContainer.WorldToLocal(windowMousePosition);
            drawState.position = new Rect(graphMousePosition, Vector2.zero);
            node.DrawState = drawState;
            
            var nodeView = new GenericNodeView();
            nodeView.Initialize(node);
            _genericGraphView.AddElement(nodeView);
        }
    }
}