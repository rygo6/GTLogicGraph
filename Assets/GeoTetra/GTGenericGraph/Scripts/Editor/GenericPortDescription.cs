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
    [Serializable]
    public abstract class GenericPortDescription
    {
        private readonly int _id;
        private readonly string _displayName = "";
        private readonly SlotType _slotType = SlotType.Input;

        public NodeDescription Owner { get; private set; }

        protected GenericPortDescription() {}

        protected GenericPortDescription(NodeDescription owner, int slotId, string displayName, SlotType slotType)
        {
            Owner = owner;
            _id = slotId;
            _displayName = displayName;
            _slotType = slotType;

        }

        static string ConcreteSlotValueTypeAsString(ConcreteSlotValueType type)
        {
            switch (type)
            {
                case ConcreteSlotValueType.Vector1:
                    return "(1)";
                case ConcreteSlotValueType.Vector2:
                    return "(2)";
                case ConcreteSlotValueType.Vector3:
                    return "(3)";
                case ConcreteSlotValueType.Vector4:
                    return "(4)";
                case ConcreteSlotValueType.Boolean:
                    return "(B)";
                case ConcreteSlotValueType.Matrix2:
                    return "(2x2)";
                case ConcreteSlotValueType.Matrix3:
                    return "(3x3)";
                case ConcreteSlotValueType.Matrix4:
                    return "(4x4)";
                case ConcreteSlotValueType.SamplerState:
                    return "(SS)";
                case ConcreteSlotValueType.Texture2D:
                    return "(T)";
                case ConcreteSlotValueType.Cubemap:
                    return "(C)";
                default:
                    return "(E)";
            }
        }

        public virtual string DisplayName
        {
            get { return _displayName + ConcreteSlotValueTypeAsString(concreteValueType); }
       }

        public int id
        {
            get { return _id; }
        }

        public bool isInputSlot
        {
            get { return _slotType == SlotType.Input; }
        }

        public bool isOutputSlot
        {
            get { return _slotType == SlotType.Output; }
        }

        public SlotType SlotType
        {
            get { return _slotType; }
        }

        public abstract SlotValueType valueType { get; }

        public abstract ConcreteSlotValueType concreteValueType { get; }

        bool IsCompatibleWithInputSlotType(SlotValueType inputType)
        {
            switch (valueType)
            {
                case SlotValueType.SamplerState:
                    return inputType == SlotValueType.SamplerState;
                case SlotValueType.DynamicMatrix:
                case SlotValueType.Matrix4:
                    return inputType == SlotValueType.Matrix4
                        || inputType == SlotValueType.Matrix3
                        || inputType == SlotValueType.Matrix2
                        || inputType == SlotValueType.DynamicMatrix
                        || inputType == SlotValueType.Dynamic;
                case SlotValueType.Matrix3:
                    return inputType == SlotValueType.Matrix3
                        || inputType == SlotValueType.Matrix2
                        || inputType == SlotValueType.DynamicMatrix
                        || inputType == SlotValueType.Dynamic;
                case SlotValueType.Matrix2:
                    return inputType == SlotValueType.Matrix2
                        || inputType == SlotValueType.DynamicMatrix
                        || inputType == SlotValueType.Dynamic;
                case SlotValueType.Texture2D:
                    return inputType == SlotValueType.Texture2D;
                case SlotValueType.Cubemap:
                    return inputType == SlotValueType.Cubemap;
                case SlotValueType.DynamicVector:
                case SlotValueType.Vector4:
                case SlotValueType.Vector3:
                case SlotValueType.Vector2:
                case SlotValueType.Vector1:
                    return inputType == SlotValueType.Vector4
                        || inputType == SlotValueType.Vector3
                        || inputType == SlotValueType.Vector2
                        || inputType == SlotValueType.Vector1
                        || inputType == SlotValueType.DynamicVector
                        || inputType == SlotValueType.Dynamic;
                case SlotValueType.Dynamic:
                    return inputType == SlotValueType.Matrix4
                        || inputType == SlotValueType.Matrix3
                        || inputType == SlotValueType.Matrix2
                        || inputType == SlotValueType.DynamicMatrix
                        || inputType == SlotValueType.Vector4
                        || inputType == SlotValueType.Vector3
                        || inputType == SlotValueType.Vector2
                        || inputType == SlotValueType.Vector1
                        || inputType == SlotValueType.DynamicVector
                        || inputType == SlotValueType.Dynamic;
                case SlotValueType.Boolean:
                    return inputType == SlotValueType.Boolean;
            }
            return false;
        }

        public bool IsCompatibleWith(GenericPortDescription otherPortDescription)
        {
            return otherPortDescription != null
                && otherPortDescription.Owner != Owner
                && otherPortDescription.isInputSlot != isInputSlot
                && ((isInputSlot
                     ? otherPortDescription.IsCompatibleWithInputSlotType(valueType)
                     : IsCompatibleWithInputSlotType(otherPortDescription.valueType)));
        }
    }
}
