using UnityEngine;
using System.IO;
using System.Text;
using GeoTetra.GTLogicGraph;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.Graphs;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[ScriptedImporter(13, LogicGraphImporter.LogicGraphExtension)]
public class LogicGraphImporter : ScriptedImporter
{
    public const string LogicGraphExtension = "LogicGraph";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var textGraph = File.ReadAllText(ctx.assetPath, Encoding.UTF8);
        var graph = JsonUtility.FromJson<LogicGraphData>(textGraph);
        LogicGraphObject logicGraphObject = ScriptableObject.CreateInstance<LogicGraphObject>();
        logicGraphObject.Initialize(graph);
        ctx.AddObjectToAsset("MainAsset", logicGraphObject);
        ctx.SetMainObject(logicGraphObject);
    }
}