using UnityEngine;
using System.IO;
using System.Text;
using GeoTetra.GTLogicGraph;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.Graphs;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[ScriptedImporter(13, LogicGraphImporter.GenericGraphExtension)]
public class LogicGraphImporter : ScriptedImporter
{
    public const string GenericGraphExtension = "GenericGraph";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var textGraph = File.ReadAllText(ctx.assetPath, Encoding.UTF8);
        var graph = JsonUtility.FromJson<GraphData>(textGraph);
        GraphLogicData graphObject = ScriptableObject.CreateInstance<GraphLogicData>();
        graphObject.Initialize(graph);
        ctx.AddObjectToAsset("MainAsset", graphObject);
        ctx.SetMainObject(graphObject);
    }
}