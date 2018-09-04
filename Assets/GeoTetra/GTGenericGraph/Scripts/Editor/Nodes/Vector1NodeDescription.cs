using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Title("Basic", "Vector1")]
    [NodeDescriptionType("Vector1")]
    public class Vector1NodeDescription : NodeDescription
    {
        [SerializeField]
        private float _value;

        [SerializeField]
        private bool _boolValue;
        
        const string kInputSlotXName = "X";
        const string kOutputSlotName = "Out";

        public const int InputSlotXId = 0;
        public const int OutputSlotId = 1;

        [NodeToggleControl("Bool")]
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
        
        public override void ConstructNode()
        {
            AddSlot(new Vector1PortDescription(this, InputSlotXId, kInputSlotXName, PortDirection.Input));
            AddSlot(new Vector1PortDescription(this, OutputSlotId, kOutputSlotName, PortDirection.Output));
            AddSlot(new BooleanPortDescription(this, 3, kInputSlotXName, PortDirection.Input));
            AddSlot(new BooleanPortDescription(this, 4, kOutputSlotName, PortDirection.Output));
        }
    }
}