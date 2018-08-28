using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Object = UnityEngine.Object;

namespace GeoTetra.GTGenericGraph
{
    public class GenericGraphEditorWindow : EditorWindow
    {
        private GraphData _graphData;

        private GenericGraphEditorView _graphEditorView;

        private GenericGraphEditorView GenericGraphEditorView
        {
            get { return _graphEditorView; }
            set
            {
                if (_graphEditorView != null)
                {
                    _graphEditorView.RemoveFromHierarchy();
                }

                _graphEditorView = value;

                if (_graphEditorView != null)
                {
                    _graphEditorView.saveRequested += UpdateAsset;
                    _graphEditorView.showInProjectRequested += PingAsset;
                    this.GetRootVisualContainer().Add(_graphEditorView);
                }
            }
        }

//        [MenuItem("Window/GenericGraph")]
        public static void CreateWindow(GraphData graphData)
        {
            GenericGraphEditorWindow window = GetWindow<GenericGraphEditorWindow>();
            Debug.Log(graphData.GetInstanceID());
            window.Initialize(graphData);
            window.wantsMouseMove = true;
            window.Show();
        }

        private void Initialize(GraphData graphData)
        {
            try
            {
                _graphData = graphData;
                GenericGraphEditorView = new GenericGraphEditorView(this, _graphData, _graphData.name)
                {
                    persistenceKey = graphData.GetInstanceID().ToString()
                };
                GenericGraphEditorView.RegisterCallback<GeometryChangedEvent>(OnPostLayout);

                titleContent = new GUIContent(_graphData.name);

                Repaint();
            }
            catch (Exception)
            {
                _graphEditorView = null;
                _graphData = null;
                throw;
            }
        }

        private void OnDisable()
        {
            GenericGraphEditorView = null;
        }

        private void OnDestroy()
        {
            GenericGraphEditorView = null;
        }

        void Update()
        {
            GenericGraphEditorView.HandleGraphChanges();
        }

        public void PingAsset()
        {
//            if (selectedGuid != null)
//            {
//                var path = AssetDatabase.GUIDToAssetPath(selectedGuid);
//                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
//                EditorGUIUtility.PingObject(asset);
//            }
        }

        public void UpdateAsset()
        {
//            if (selectedGuid != null && GraphLogic != null)
//            {
//                var path = AssetDatabase.GUIDToAssetPath(selectedGuid);
//                if (string.IsNullOrEmpty(path) || GraphLogic == null)
//                    return;
//
//                if (_graphLogic.graph.GetType() == typeof(GenericGraph))
//                    UpdateShaderGraphOnDisk(path);
//
//                GraphLogic.isDirty = false;
//            }
        }

        void UpdateShaderGraphOnDisk(string path)
        {
//            var graph = GraphLogic.graph as IGraph;
//            if (graph == null)
//                return;
//
//            UpdateShaderGraphOnDisk(path, graph);
        }

        static void UpdateShaderGraphOnDisk(string path, IGraph graph)
        {
            File.WriteAllText(path, EditorJsonUtility.ToJson(graph, true));
            AssetDatabase.ImportAsset(path);

            Debug.Log("Saving to disk " + path + " " + graph);
        }

        void OnPostLayout(GeometryChangedEvent evt)
        {
            Debug.Log("OnGeometryChanged");
            GenericGraphEditorView.UnregisterCallback<GeometryChangedEvent>(OnPostLayout);
            GenericGraphEditorView.GenericGraphView.FrameAll();
        }
    }
}