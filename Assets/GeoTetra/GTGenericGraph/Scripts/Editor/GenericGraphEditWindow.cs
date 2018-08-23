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
    public class GenericGraphEditWindow : EditorWindow
    {
        private GenericGraph _graph;

        GenericGraph Graph
        {
            get { return _graph; }
            set
            {
                if (_graph != null)
                    DestroyImmediate(_graph);
                _graph = value;
            }
        }

        private GenericGraphEditorView m_GraphEditorView;

        private GenericGraphEditorView graphEditorView
        {
            get { return m_GraphEditorView; }
            set
            {
                if (m_GraphEditorView != null)
                {
                    m_GraphEditorView.RemoveFromHierarchy();
                }

                m_GraphEditorView = value;

                if (m_GraphEditorView != null)
                {
                    m_GraphEditorView.saveRequested += UpdateAsset;
                    m_GraphEditorView.showInProjectRequested += PingAsset;
                    this.GetRootVisualContainer().Add(m_GraphEditorView);
                }
            }
        }

//        [MenuItem("Window/GenericGraph")]
        public static void CreateWindow(GenericGraph graph)
        {
            GenericGraphEditWindow window = GetWindow<GenericGraphEditWindow>();
            Debug.Log(graph.GetInstanceID());
            window.Initialize(graph);
            window.wantsMouseMove = true;
            window.Show();
        }

        private void Initialize(GenericGraph graph)
        {
            try
            {
                Graph = graph;
                graphEditorView = new GenericGraphEditorView(this, Graph, Graph.name)
                {
                    persistenceKey = graph.GetInstanceID().ToString()
                };
                graphEditorView.RegisterCallback<GeometryChangedEvent>(OnPostLayout);

                titleContent = new GUIContent(Graph.name);

                Repaint();
            }
            catch (Exception)
            {
                m_GraphEditorView = null;
                Graph = null;
                throw;
            }
        }

        private void OnDisable()
        {
            graphEditorView = null;
        }

        private void OnDestroy()
        {
            graphEditorView = null;
        }

        void Update()
        {
//            if (m_HasError)
//                return;
//
//            if (PlayerSettings.colorSpace != m_ColorSpace)
//            {
//                graphEditorView = null;
//                m_ColorSpace = PlayerSettings.colorSpace;
//            }
//
//            try
//            {
//                if (GraphLogic == null && selectedGuid != null)
//                {
//                    var guid = selectedGuid;
//                    selectedGuid = null;
//                    Initialize(guid);
//                }
//
//                if (GraphLogic == null)
//                {
//                    Close();
//                    return;
//                }
//
//                var materialGraph = GraphLogic.graph as AbstractGenericGraph;
//                if (materialGraph == null)
//                    return;
//                if (graphEditorView == null)
//                {
//                    graphEditorView = new GenericGraphEditorView(this, materialGraph, "temp")
//                    {
//                        persistenceKey = selectedGuid
//                    };
//                    m_ColorSpace = PlayerSettings.colorSpace;
//                }
//
//                graphEditorView.HandleGraphChanges();
//                GraphLogic.graph.ClearChanges();
//            }
//            catch (Exception e)
//            {
//                m_HasError = true;
//                m_GraphEditorView = null;
//                GraphLogic = null;
//                Debug.LogException(e);
//                throw;
//            }
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
            graphEditorView.UnregisterCallback<GeometryChangedEvent>(OnPostLayout);
            graphEditorView.GenericGraphView.FrameAll();
        }
    }
}