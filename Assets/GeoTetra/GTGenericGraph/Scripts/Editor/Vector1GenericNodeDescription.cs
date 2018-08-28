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
    public class Vector1GenericNodeDescription : NodeDescription
    {
        [SerializeField]
        private float _value;

        [SerializeField]
        private bool _boolValue;
        
        const string kInputSlotXName = "X";
        const string kOutputSlotName = "Out";

        public const int InputSlotXId = 0;
        public const int OutputSlotId = 1;

        [GenericToggleControlAttribute("Two Sided")]
        public GenericToggleData BoolValue
        {
            get { return new GenericToggleData(_boolValue); }
            set
            {
                if (_boolValue == value.isOn)
                    return;
                _boolValue = value.isOn;
                SetDirty();
            }
        }
        
        public override string NodeType()
        {
            return "Vector1";
        }
        
        public override void ConstructNode()
        {
            AddSlot(new Vector1GenericPortDescription(this, InputSlotXId, kInputSlotXName, SlotType.Input));
            AddSlot(new Vector1GenericPortDescription(this, OutputSlotId, kOutputSlotName, SlotType.Output));
            AddSlot(new BooleanGenericPortDescription(this, 3, kInputSlotXName, SlotType.Input));
            AddSlot(new Vector1GenericPortDescription(this, 4, kInputSlotXName, SlotType.Input));
            
        }
    }
}