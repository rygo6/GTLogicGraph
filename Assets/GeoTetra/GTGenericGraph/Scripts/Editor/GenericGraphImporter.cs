using UnityEngine;
using System.IO;
using System.Text;
using GeoTetra.GTGenericGraph;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.Graphs;

[ScriptedImporter(13, GenericGraphImporter.GenericGraphExtension)]
public class GenericGraphImporter : ScriptedImporter
{
    public const string GenericGraphExtension = "GenericGraph";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var textGraph = File.ReadAllText(ctx.assetPath, Encoding.UTF8);
        var graph = JsonUtility.FromJson<GraphData>(textGraph);
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath(ctx.assetPath);

//        if (asset.Length == 0)
//        {
            GraphLogicData graphObject = ScriptableObject.CreateInstance<GraphLogicData>();
            graphObject.Initialize(graph);
            ctx.AddObjectToAsset("MainAsset", graphObject);
            ctx.SetMainObject(graphObject);
//        }
//        else
//        {
//            Debug.Log("updating existing object");
//            GraphLogicData graphObject = asset[0]  as GraphLogicData;
//            graphObject.Initialize(graph);
//        }
    }
}