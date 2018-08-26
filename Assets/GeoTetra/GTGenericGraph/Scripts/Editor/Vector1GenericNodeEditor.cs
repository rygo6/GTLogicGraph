using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Title("Basic", "Vector1")]
    [NodeType("Vector1")]
    public class Vector1GenericNodeEditor : NodeEditor
    {
        [SerializeField]
        private float _value;
        
        private const string ValueName = "_value";
        
        const string kInputSlotXName = "X";
        const string kOutputSlotName = "Out";

        public const int InputSlotXId = 0;
        public const int OutputSlotId = 1;

        public override string NodeType()
        {
            return "Vector1";
        }
        
        public override void ConstructNode()
        {
            AddSlot(new Vector1GenericSlot(this, InputSlotXId, kInputSlotXName, SlotType.Input, _value));
            AddSlot(new Vector1GenericSlot(this, OutputSlotId, kOutputSlotName, SlotType.Output, 0));
//            AddSlot(new BooleanGenericSlot(3, kInputSlotXName, SlotType.Input, null));
//            AddSlot(new Vector1GenericSlot(4, kInputSlotXName, SlotType.Input, null));
        }
    }
}