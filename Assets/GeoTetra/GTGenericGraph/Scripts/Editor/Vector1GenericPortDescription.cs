using System;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph.Slots
{
    [Serializable]
    public class Vector1GenericPortDescription : GenericPortDescription
    {
        public override PortValueType ValueType { get { return PortValueType.Vector1; } }

        public Vector1GenericPortDescription(NodeDescription owner, int slotId, string displayName,
            PortDirection portDirection) : base(owner, slotId, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Vector1;
        }
    }
}
