using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Title("Basic", "Vector1")]
    [NodeEditorType(typeof(Vector1LogicNode))]
    public class Vector1NodeEditor : NodeEditor
    {
        [SerializeField]
        private float _value;

        [SerializeField]
        private bool _boolValue;

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
            AddSlot(new Vector1PortDescription(this, "Vector1Input", "X", PortDirection.Input));
            AddSlot(new Vector1PortDescription(this, "Vector1Output", "Out", PortDirection.Output));
//            AddSlot(new BooleanPortDescription(this, 3, InputPortDisplayName, PortDirection.Input));
//            AddSlot(new BooleanPortDescription(this, 4, OutputPortDisplayName, PortDirection.Output));
        }
    }
}