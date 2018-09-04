using UnityEngine;
using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;

namespace GeoTetra.GTGenericGraph
{
    [Title("Input", "Vector1")]
    [NodeDescriptionType("Vector1Input")]
    public class Vector1InputNodeDescription : NodeDescription, IInputNode
    {
        [SerializeField] 
        private float _value;
        
        private const string ValueName = "_value";

        private const string InputSlotXName = "X";
        private const int InputSlotId = 0;

        [VectorControlAttribute("X", "Y", "Z", "U", "V ")]
        public Vector3 Value2 { get; set; }

        
        [VectorControlAttribute("Value", "X", "Y", "Z", "W")]
        public float Value
        {
            get { return _value; }
            set { _value = value; }
        }
        
        public override void ConstructNode()
        {
            AddSlot(new Vector1PortDescription(this, InputSlotId, InputSlotXName, PortDirection.Output));
        }
    }
}

