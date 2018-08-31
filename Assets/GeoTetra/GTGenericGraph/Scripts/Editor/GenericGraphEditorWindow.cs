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
        private GraphObject _graphObject;

        private GenericGraphEditorView _graphEditorView;
        private string _selectedGuid;

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

        public string SelectedGuid
        {
            get { return _selectedGuid; }
        }

        public void Initialize(string guid)
        {
            try
            {
                _selectedGuid = guid;
                var asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
                var path = AssetDatabase.GetAssetPath(asset);
                var textGraph = File.ReadAllText(path, Encoding.UTF8);

                _graphObject = CreateInstance<GraphObject>();
                GraphData graphData = JsonUtility.FromJson<GraphData>(textGraph);
                _graphObject.Initialize(graphData);
                GenericGraphEditorView = new GenericGraphEditorView(this, _graphObject)
                {
                    persistenceKey = _graphObject.GetInstanceID().ToString()
                };
                GenericGraphEditorView.RegisterCallback<GeometryChangedEvent>(OnPostLayout);

                titleContent = new GUIContent(_graphObject.name);

                Repaint();
            }
            catch (Exception)
            {
                _graphEditorView = null;
                _graphObject = null;
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
            if (SelectedGuid != null && _graphObject != null)
            {
                var path = AssetDatabase.GUIDToAssetPath(SelectedGuid);
                if (string.IsNullOrEmpty(path) && _graphObject.GraphData != null)
                    return;

                var shaderImporter = AssetImporter.GetAtPath(path) as GenericGraphImporter;
                if (shaderImporter == null)
                    return;
                
                File.WriteAllText(path, EditorJsonUtility.ToJson(_graphObject.GraphData, true));
                shaderImporter.SaveAndReimport();
                AssetDatabase.ImportAsset(path);
            }
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