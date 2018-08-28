using UnityEngine;
using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;

namespace GeoTetra.GTGenericGraph
{
    [Title("Output", "Vector1")]
    public class Vector1OutputNodeDescription : NodeDescription
    {
        private const string ValueName = "_value";

        private const string InputSlotXName = "X";
        private const int InputSlotId = 0;

        public override void ConstructNode()
        {
            AddSlot(new Vector1GenericPortDescription(this, InputSlotId, InputSlotXName, SlotType.Input));
        }

        public override string NodeType()
        {
            return "Vector 1 Output";
        }

    }
}

