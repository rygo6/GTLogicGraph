using UnityEditor.ShaderGraph;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeoTetra.GTGenericGraph;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.ShaderGraph.Drawing;

[ScriptedImporter(13, GenericGraphImporter.GenericGraphExtension)]
public class GenericGraphImporter : ScriptedImporter
{
    public const string GenericGraphExtension = "GenericGraph";

    public override void OnImportAsset(AssetImportContext ctx)
    {
        var textGraph = File.ReadAllText(ctx.assetPath, Encoding.UTF8);
        var graph = JsonUtility.FromJson<GraphData>(textGraph);
//        graph.LoadedFromDisk();
//        graph.GetLogicChains();
        
        GameObject graphGameObject = new GameObject("GenericGraph");
        ctx.AddObjectToAsset("MainAsset", graphGameObject);
        ctx.SetMainObject(graphGameObject);
    }
}