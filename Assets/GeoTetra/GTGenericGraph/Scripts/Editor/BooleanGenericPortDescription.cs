using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public class BooleanGenericPortDescription : GenericPortDescription
    {
        public override PortValueType ValueType { get { return PortValueType.Boolean; } }

        public BooleanGenericPortDescription(NodeDescription owner, int slotId, string displayName,
            PortDirection portDirection) : base(owner, slotId, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Boolean;
        }
    }
}