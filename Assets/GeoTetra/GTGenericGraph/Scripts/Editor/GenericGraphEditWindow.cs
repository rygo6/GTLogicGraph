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
        [SerializeField] GraphObject m_GraphObject;

        [SerializeField] private string m_Selected;

        [NonSerialized] bool m_HasError;

        ColorSpace m_ColorSpace;

        GraphObject graphObject
        {
            get { return m_GraphObject; }
            set
            {
                if (m_GraphObject != null)
                    DestroyImmediate(m_GraphObject);
                m_GraphObject = value;
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
//                    _builderGraphView.Dispose();
                }

                m_GraphEditorView = value;

                if (m_GraphEditorView != null)
                {
                    m_GraphEditorView.saveRequested += UpdateAsset;
//                    _builderGraphView.convertToSubgraphRequested += ToSubGraph;//
                    m_GraphEditorView.showInProjectRequested += PingAsset;
                    this.GetRootVisualContainer().Add(m_GraphEditorView);
                }
            }
        }

        public string selectedGuid
        {
            get { return m_Selected; }
            private set { m_Selected = value; }
        }

//        [MenuItem("Window/GenericGraph")]
        public static void CreateWindow(string guid)
        {
            GenericGraphEditWindow window = GetWindow<GenericGraphEditWindow>();
            Debug.Log(guid);
            window.Initialize(guid);
            window.wantsMouseMove = true;
            window.Show();
        }

        private void Initialize(string assetGuid)
        {
            try
            {
                m_ColorSpace = PlayerSettings.colorSpace;

                var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(assetGuid));
                if (asset == null)
                    return;

                if (!EditorUtility.IsPersistent(asset))
                    return;

                if (selectedGuid == assetGuid)
                    return;

                var path = AssetDatabase.GetAssetPath(asset);
                var extension = Path.GetExtension(path);
                Type graphType;
                switch (extension)
                {
                    case ".GenericGraph":
                        graphType = typeof(GenericGraph);
                        break;
                    case ".GenericSubGraph":
                        graphType = typeof(SubGraph);
                        break;
                    default:
                        return;
                }

                selectedGuid = assetGuid;

                var textGraph = File.ReadAllText(path, Encoding.UTF8);
                graphObject = CreateInstance<GraphObject>();
                graphObject.hideFlags = HideFlags.HideAndDontSave;
                graphObject.graph = JsonUtility.FromJson(textGraph, graphType) as IGraph;
//                graphObject.graph = new GenericGraph();
                graphObject.graph.OnEnable();
                graphObject.graph.ValidateGraph();

                graphEditorView =
                    new GenericGraphEditorView(this, m_GraphObject.graph as AbstractGenericGraph, asset.name)
                    {
                        persistenceKey = selectedGuid
                    };
//                new GenericGraphEditorView(this, m_GraphObject.graph as AbstractGenericGraph, "temp")
//                {
//                    persistenceKey = selectedGuid
//                };
                graphEditorView.RegisterCallback<GeometryChangedEvent>(OnPostLayout);

                titleContent = new GUIContent(asset.name);
//                titleContent = new GUIContent("temp");

                Repaint();
            }
            catch (Exception)
            {
                m_HasError = true;
                m_GraphEditorView = null;
                graphObject = null;
                throw;
            }
        }

        private void OnDisable()
        {
            graphEditorView = null;
        }

        private void OnDestroy()
        {
            if (graphObject != null)
            {
//                if (graphObject.isDirty && EditorUtility.DisplayDialog("Shader Graph Has Been Modified", "Do you want to save the changes you made in the shader graph?\n\nYour changes will be lost if you don't save them.", "Save", "Don't Save"))
//                    UpdateAsset();
                Undo.ClearUndo(graphObject);
                DestroyImmediate(graphObject);
            }

            graphEditorView = null;
        }

        void Update()
        {
            if (m_HasError)
                return;

            if (PlayerSettings.colorSpace != m_ColorSpace)
            {
                graphEditorView = null;
                m_ColorSpace = PlayerSettings.colorSpace;
            }

            try
            {
                if (graphObject == null && selectedGuid != null)
                {
                    var guid = selectedGuid;
                    selectedGuid = null;
                    Initialize(guid);
                }

                if (graphObject == null)
                {
                    Close();
                    return;
                }

                var materialGraph = graphObject.graph as AbstractGenericGraph;
                if (materialGraph == null)
                    return;
                if (graphEditorView == null)
                {
//                    var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(selectedGuid));
                    graphEditorView =
                        new GenericGraphEditorView(this, materialGraph, "temp") {persistenceKey = selectedGuid};
                    m_ColorSpace = PlayerSettings.colorSpace;
                }

                graphEditorView.HandleGraphChanges();
                graphObject.graph.ClearChanges();
            }
            catch (Exception e)
            {
                m_HasError = true;
                m_GraphEditorView = null;
                graphObject = null;
                Debug.LogException(e);
                throw;
            }
        }

        public void PingAsset()
        {
            if (selectedGuid != null)
            {
                var path = AssetDatabase.GUIDToAssetPath(selectedGuid);
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                EditorGUIUtility.PingObject(asset);
            }
        }

        public void UpdateAsset()
        {
            if (selectedGuid != null && graphObject != null)
            {
                var path = AssetDatabase.GUIDToAssetPath(selectedGuid);
                if (string.IsNullOrEmpty(path) || graphObject == null)
                    return;

                if (m_GraphObject.graph.GetType() == typeof(GenericGraph))
                    UpdateShaderGraphOnDisk(path);

//                if (m_GraphObject.graph.GetType() == typeof(SubGraph))
//                    UpdateAbstractSubgraphOnDisk<SubGraph>(path);

                graphObject.isDirty = false;
//                var windows = Resources.FindObjectsOfTypeAll<GenericGraphEditWindow>();
//                foreach (var materialGraphEditWindow in windows)
//                {
//                    materialGraphEditWindow.Rebuild();
//                }
            }
        }

        void UpdateShaderGraphOnDisk(string path)
        {
            var graph = graphObject.graph as IGraph;
            if (graph == null)
                return;

            UpdateShaderGraphOnDisk(path, graph);
        }

        static void UpdateShaderGraphOnDisk(string path, IGraph graph)
        {
//            var shaderImporter = AssetImporter.GetAtPath(path) as ShaderGraphImporter;
//            if (shaderImporter == null)
//                return;

            File.WriteAllText(path, EditorJsonUtility.ToJson(graph, true));
//            shaderImporter.SaveAndReimport();
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