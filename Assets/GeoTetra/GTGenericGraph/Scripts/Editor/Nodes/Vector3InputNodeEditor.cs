using UnityEngine;
using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;

namespace GeoTetra.GTGenericGraph
{
    [Title("Input", "Vector3")]
    [NodeEditorType(typeof(Vector3InputLogicNode))]
    public class Vector3InputNodeEditor : NodeEditor, IInputNode
    {        
        public override void ConstructNode()
        {
            AddSlot(new Vector3PortDescription(this, "Vector3Output", "Out", PortDirection.Output));
        }
    }
}

