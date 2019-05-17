using System;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    public abstract class LogicSlot
    {
        private readonly string _memberName;
        private readonly string _displayName = "";
        private readonly SlotDirection _direction;

        public AbstractLogicNodeEditor Owner { get; }
        
        public LogicSlot(AbstractLogicNodeEditor owner, string memberName, string displayName, SlotDirection direction)
        {
            Owner = owner;
            _memberName = memberName;
            _displayName = displayName;
            _direction = direction;
        }

        public string DisplayName => _displayName + " " + ValueType;

        public string MemberName => _memberName;

        public bool isInputSlot => _direction == SlotDirection.Input;

        public bool isOutputSlot => _direction == SlotDirection.Output;

        public SlotDirection Direction => _direction;

        public abstract SlotValueType ValueType { get; }

        public abstract bool IsCompatibleWithInputSlotType(SlotValueType inputType);

        public bool IsCompatibleWith(LogicSlot otherLogicSlot)
        {
            return otherLogicSlot != null
                   && otherLogicSlot.Owner != Owner
                   && otherLogicSlot.isInputSlot != isInputSlot
                   && ((isInputSlot
                       ? otherLogicSlot.IsCompatibleWithInputSlotType(ValueType)
                       : IsCompatibleWithInputSlotType(otherLogicSlot.ValueType)));
        }
        
        public virtual VisualElement InstantiateControl()
        {
            return null;
        }
    }

    public enum SlotDirection
    {
        Input,
        Output
    }
    
    [Serializable]
    public enum SlotValueType
    {
        //TODO should these be strings?
        Vector4,
        Vector3,
        Vector2,
        Vector1,
        Boolean,
        CurvePrimitive,
        Mesh,
        VertexList,
    }
}