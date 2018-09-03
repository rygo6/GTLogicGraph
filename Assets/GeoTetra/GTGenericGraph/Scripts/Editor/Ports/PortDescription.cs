using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GeoTetra.GTGenericGraph.Slots;
using UnityEngine;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
    public abstract class PortDescription
    {
        private readonly int _id;
        private readonly string _displayName = "";
        private readonly PortDirection _portDirection;

        public NodeDescription Owner { get; private set; }
        
        public PortDescription(NodeDescription owner, int slotId, string displayName, PortDirection portDirection)
        {
            Owner = owner;
            _id = slotId;
            _displayName = displayName;
            _portDirection = portDirection;
        }

        public string DisplayName
        {
            get { return _displayName + " " + ValueType; }
        }

        public int id
        {
            get { return _id; }
        }

        public bool isInputSlot
        {
            get { return _portDirection == PortDirection.Input; }
        }

        public bool isOutputSlot
        {
            get { return _portDirection == PortDirection.Output; }
        }

        public PortDirection PortDirection
        {
            get { return _portDirection; }
        }

        public abstract PortValueType ValueType { get; }

        public abstract bool IsCompatibleWithInputSlotType(PortValueType inputType);

        public bool IsCompatibleWith(PortDescription otherPortDescription)
        {
            return otherPortDescription != null
                   && otherPortDescription.Owner != Owner
                   && otherPortDescription.isInputSlot != isInputSlot
                   && ((isInputSlot
                       ? otherPortDescription.IsCompatibleWithInputSlotType(ValueType)
                       : IsCompatibleWithInputSlotType(otherPortDescription.ValueType)));
        }
    }

    public enum PortDirection
    {
        Input,
        Output
    }
    
    [Serializable]
    public enum PortValueType
    {
        SamplerState,
        DynamicMatrix,
        Matrix4,
        Matrix3,
        Matrix2,
        Texture2D,
        Cubemap,
        DynamicVector,
        Vector4,
        Vector3,
        Vector2,
        Vector1,
        Dynamic,
        Boolean
    }
}