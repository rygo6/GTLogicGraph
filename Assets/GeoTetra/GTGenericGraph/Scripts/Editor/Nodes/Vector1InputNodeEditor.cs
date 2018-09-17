using UnityEngine;
using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;

namespace GeoTetra.GTGenericGraph
{
    [Title("Input", "Vector1")]
    [NodeEditorType(typeof(Vector1InputLogicNode))]
    public class Vector1InputNodeEditor : NodeEditor, IInputNode
    {        
        public override void ConstructNode()
        {
            AddSlot(new Vector1PortDescription(this, "Vector1Output", "X", PortDirection.Output));
        }
    }
}

