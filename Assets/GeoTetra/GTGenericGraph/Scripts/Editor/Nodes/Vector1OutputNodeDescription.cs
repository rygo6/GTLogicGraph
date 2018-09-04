using UnityEngine;
using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;

namespace GeoTetra.GTGenericGraph
{
    [Title("Output", "Vector1")]
    [NodeDescriptionType("Vector1Output")]
    public class Vector1OutputNodeDescription : NodeDescription, IOutputNode
    {
        private const string ValueName = "_value";

        private const string InputSlotXName = "X";
        private const int InputSlotId = 0;

        public override void ConstructNode()
        {
            AddSlot(new Vector1PortDescription(this, InputSlotId, InputSlotXName, PortDirection.Input));
        }
    }
}

