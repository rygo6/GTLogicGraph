using UnityEngine;
using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;

namespace GeoTetra.GTGenericGraph
{
    [Title("Output", "Vector1")]
    [NodeEditorType(typeof(Vector1OutputLogicNode))]
    public class Vector1OutputNodeEditor : NodeEditor, IOutputNode
    {
        public override void ConstructNode()
        {
            AddSlot(new Vector1PortDescription(this, "Vector1Input", "X", PortDirection.Input));
        }
    }
}

