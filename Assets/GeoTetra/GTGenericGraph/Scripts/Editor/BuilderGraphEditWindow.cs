using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
    public class BuilderGraphEditWindow : EditorWindow
    {
        [SerializeField]
        private string _selected;

        private GenericGraphEditorView _genericGraphEditorView;

        private GenericGraphEditorView GenericGraphEditorView
        {
            get { return _genericGraphEditorView; }
            set
            {
                if (_genericGraphEditorView != null)
                {
                    _genericGraphEditorView.RemoveFromHierarchy();
//                    _builderGraphView.Dispose();
                }

                _genericGraphEditorView = value;

                if (_genericGraphEditorView != null)
                {
//                    _builderGraphView.saveRequested += UpdateAsset;//
//                    _builderGraphView.convertToSubgraphRequested += ToSubGraph;//
//                    _builderGraphView.showInProjectRequested += PingAsset;
                    this.GetRootVisualContainer().Add(_genericGraphEditorView);
                }
            }
        }

        public string SelectedGuid
        {
            get { return _selected; }
            private set { _selected = value; }
        }

        [MenuItem("Window/GenericGraph")]
        private static void CreateFromMenu()
        {
            BuilderGraphEditWindow window = GetWindow<BuilderGraphEditWindow>();
            window.Initialize();
            window.wantsMouseMove = true;
            window.Show();
        }

        public void Initialize()
        {
            Debug.Log("Initializing Window.");
            GenericGraphEditorView = new GenericGraphEditorView(this, "") {persistenceKey = SelectedGuid};
            GenericGraphEditorView.RegisterCallback<PostLayoutEvent>(OnPostLayout);
//            BuilderGraphView.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }
        
        private void OnDisable()
        {
            GenericGraphEditorView = null;
        }

        private void OnDestroy()
        {
//            if (graphObject != null)
//            {
//                if (graphObject.isDirty && EditorUtility.DisplayDialog("Shader Graph Has Been Modified", "Do you want to save the changes you made in the shader graph?\n\nYour changes will be lost if you don't save them.", "Save", "Don't Save"))
//                    UpdateAsset();
//                Undo.ClearUndo(graphObject);
//                DestroyImmediate(graphObject);
//            }

            GenericGraphEditorView = null;
        }

        private void Update()
        {
//            _builderGraphView.HandleGraphChanges();
        }

//        private void OnMouseDown(MouseDownEvent evt)
//        {
//            Debug.Log("Mouse Down " + evt);
//        }
//
        private void OnPostLayout(PostLayoutEvent evt)
        {
            Debug.Log("OnGeometryChanged");
            GenericGraphEditorView.UnregisterCallback<PostLayoutEvent>(OnPostLayout);
            GenericGraphEditorView.GenericGraphView.FrameAll();
        }
    }
}