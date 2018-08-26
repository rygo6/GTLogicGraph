using UnityEngine;
using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;

namespace GeoTetra.GTGenericGraph
{
    [Title("Output", "Vector1")]
    public class Vector1OutputNodeEditor : NodeEditor
    {
        private const string ValueName = "_value";

        private const string InputSlotXName = "X";
        private const int InputSlotId = 0;

        public override void ConstructNode()
        {
            AddSlot(new Vector1GenericSlot(this, InputSlotId, InputSlotXName, SlotType.Input, 0));
        }

        public override string NodeType()
        {
            return "Vector 1 Output";
        }

    }
}

