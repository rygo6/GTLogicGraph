using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using GeoTetra.GTLogicGraph;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.Graphs;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEngine.SceneManagement;

[ScriptedImporter(13, LogicGraphImporter.LogicGraphExtension)]
public class LogicGraphImporter : ScriptedImporter
{
    public const string LogicGraphExtension = "LogicGraph";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        List<Object> objectList = new List<Object>();
        Debug.Log("MAIN OBJECT  " + ctx.mainObject);
        ctx.GetObjects(objectList);
        Debug.Log(" OBJECTS  " + objectList.Count);

        
        var textGraph = File.ReadAllText(ctx.assetPath, Encoding.UTF8);
        LogicGraphObject loadedGraphObject = AssetDatabase.LoadAssetAtPath<LogicGraphObject>(ctx.assetPath);
        if (loadedGraphObject == null)
        {
            Debug.Log("Generating new");
            var graph = JsonUtility.FromJson<LogicGraphData>(textGraph);
            LogicGraphObject logicGraphObject = ScriptableObject.CreateInstance<LogicGraphObject>();
            logicGraphObject.Initialize(graph);
            ctx.AddObjectToAsset("MainAsset", logicGraphObject);
            ctx.SetMainObject(logicGraphObject);
        }
        else
        {
            Debug.Log("Updating Old");
            JsonUtility.FromJsonOverwrite(textGraph, loadedGraphObject.GraphData);
            ctx.AddObjectToAsset("MainAsset", loadedGraphObject);
            ctx.SetMainObject(loadedGraphObject);
        }
        
        Debug.Log(loadedGraphObject);

//        AssetDatabase.SaveAssets();
//        EditorSceneManager.SaveOpenScenes();
        


        Debug.Log("Set Asset");

//        AssetDatabase.Refresh();
    }
}