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
    public abstract class GenericSlot
    {
        const string NotInit =  "Not Initilaized";

        [SerializeField]
        int _id;

        [SerializeField]
        string _displayName = NotInit;

        [SerializeField]
        SlotType _slotType = SlotType.Input;

        [SerializeField]
        bool _hidden;

        bool _hasError;

        protected GenericSlot() {}

        protected GenericSlot(int slotId, string displayName, SlotType slotType, bool hidden = false)
        {
            _id = slotId;
            _displayName = displayName;
            _slotType = slotType;
            _hidden = hidden;
        }

        public virtual VisualElement InstantiateControl()
        {
            return null;
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

        public virtual string displayName
        {
            get { return _displayName + ConcreteSlotValueTypeAsString(concreteValueType); }
            set { _displayName = value; }
        }

        public string RawDisplayName()
        {
            return _displayName;
        }

        public static GenericSlot CreateGenericSlot(SlotValueType type, int slotId, string displayName, string shaderOutputName, SlotType slotType, Vector4 defaultValue, ShaderStage shaderStage = ShaderStage.Dynamic, bool hidden = false)
        {
            switch (type)
            {
//                case SlotValueType.SamplerState:
//                    return new SamplerStateMaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
//                case SlotValueType.DynamicMatrix:
//                    return new DynamicMatrixMaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
//                case SlotValueType.Matrix4:
//                    return new Matrix4MaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
//                case SlotValueType.Matrix3:
//                    return new Matrix3MaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
//                case SlotValueType.Matrix2:
//                    return new Matrix2MaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
//                case SlotValueType.Texture2D:
//                    return slotType == SlotType.Input
//                        ? new Texture2DInputMaterialSlot(slotId, displayName, shaderOutputName, shaderStage, hidden)
//                        : new Texture2DMaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
//                case SlotValueType.Cubemap:
//                    return slotType == SlotType.Input
//                        ? new CubemapInputMaterialSlot(slotId, displayName, shaderOutputName, shaderStage, hidden)
//                        : new CubemapMaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
//                case SlotValueType.Gradient:
//                    return slotType == SlotType.Input
//                        ? new GradientInputMaterialSlot(slotId, displayName, shaderOutputName, shaderStage, hidden)
//                        : new GradientMaterialSlot(slotId, displayName, shaderOutputName, slotType, shaderStage, hidden);
//                case SlotValueType.DynamicVector:
//                    return new DynamicVectorMaterialSlot(slotId, displayName, shaderOutputName, slotType, defaultValue, shaderStage, hidden);
//                case SlotValueType.Vector4:
//                    return new Vector4MaterialSlot(slotId, displayName, shaderOutputName, slotType, defaultValue, shaderStage, hidden: hidden);
//                case SlotValueType.Vector3:
//                    return new Vector3MaterialSlot(slotId, displayName, shaderOutputName, slotType, defaultValue, shaderStage, hidden: hidden);
//                case SlotValueType.Vector2:
//                    return new Vector2MaterialSlot(slotId, displayName, shaderOutputName, slotType, defaultValue, shaderStage, hidden: hidden);
//                case SlotValueType.Vector1:
//                    return new Vector1MaterialSlot(slotId, displayName, shaderOutputName, slotType, defaultValue.x, shaderStage, hidden: hidden);
//                case SlotValueType.Dynamic:
//                    return new DynamicValueMaterialSlot(slotId, displayName, shaderOutputName, slotType, new Matrix4x4(defaultValue, Vector4.zero, Vector4.zero, Vector4.zero), shaderStage, hidden);
//                case SlotValueType.Boolean:
//                    return new BooleanMaterialSlot(slotId, displayName, shaderOutputName, slotType, false, shaderStage, hidden);
                case SlotValueType.Vector1:
                    return new Vector1GenericSlot(slotId, displayName, shaderOutputName, slotType, defaultValue.x, hidden: hidden);
            }

            throw new ArgumentOutOfRangeException("type", type, null);
        }

        public NodeEditor Owner { get; set; }

        public bool hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
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

        public bool hasError
        {
            get { return _hasError; }
            set { _hasError = value; }
        }

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

        public bool IsCompatibleWith(GenericSlot otherSlot)
        {
            return otherSlot != null
                && otherSlot.Owner != Owner
                && otherSlot.isInputSlot != isInputSlot
                && ((isInputSlot
                     ? otherSlot.IsCompatibleWithInputSlotType(valueType)
                     : IsCompatibleWithInputSlotType(otherSlot.valueType)));
        }

        protected virtual string ConcreteSlotValueAsVariable(AbstractMaterialNode.OutputPrecision precision)
        {
            return "error";
        }

        protected static PropertyType ConvertConcreteSlotValueTypeToPropertyType(ConcreteSlotValueType slotValue)
        {
            switch (slotValue)
            {
                case ConcreteSlotValueType.Texture2D:
                    return PropertyType.Texture;
                case ConcreteSlotValueType.Cubemap:
                    return PropertyType.Cubemap;
                case ConcreteSlotValueType.Boolean:
                    return PropertyType.Boolean;
                case ConcreteSlotValueType.Vector1:
                    return PropertyType.Vector1;
                case ConcreteSlotValueType.Vector2:
                    return PropertyType.Vector2;
                case ConcreteSlotValueType.Vector3:
                    return PropertyType.Vector3;
                case ConcreteSlotValueType.Vector4:
                    return PropertyType.Vector4;
                case ConcreteSlotValueType.Matrix2:
                    return PropertyType.Matrix2;
                case ConcreteSlotValueType.Matrix3:
                    return PropertyType.Matrix3;
                case ConcreteSlotValueType.Matrix4:
                    return PropertyType.Matrix4;
                case ConcreteSlotValueType.SamplerState:
                    return PropertyType.SamplerState;
                default:
                    return PropertyType.Vector4;
            }
        }

        public abstract void CopyValuesFrom(GenericSlot foundSlot);

//        bool Equals(GenericSlot other)
//        {
//            return _id == other._id && owner.guid.Equals(other.owner.guid);
//        }

        public bool Equals(ISlot other)
        {
            return Equals(other as object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GenericSlot)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_id * 397) ^ (Owner != null ? Owner.GetHashCode() : 0);
            }
        }
    }
}
