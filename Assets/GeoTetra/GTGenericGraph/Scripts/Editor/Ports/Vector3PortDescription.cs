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
    public class Vector3PortDescription : PortDescription
    {
        public override PortValueType ValueType
        {
            get { return PortValueType.Vector3; }
        }

        public Vector3PortDescription(NodeEditor owner, string memberName, string displayName, PortDirection portDirection) 
            : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Vector3;
        }
    }
}
