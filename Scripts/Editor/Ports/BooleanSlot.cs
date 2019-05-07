using System;
using GeoTetra.GTCommon;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    [Serializable]
    public class BooleanSlot : LogicSlot
    {
        private readonly string[] _labels;
        private readonly Func<Bool4> _get;
        private readonly Action<Bool4> _set;
        
        public override SlotValueType ValueType { get { return SlotValueType.Boolean; } }

        public BooleanSlot(AbstractLogicNodeEditor owner, 
            string memberName, 
            string displayName,
            SlotDirection direction,
            string[] labels,
            Func<Bool4> get,
            Action<Bool4> set) : base(owner, memberName, displayName, direction)
        {
            _labels = labels;
            _get = get;
            _set = set;
        }

        public override bool IsCompatibleWithInputSlotType(SlotValueType inputType)
        {
            return inputType == SlotValueType.Boolean;
        }
        
        public override VisualElement InstantiateControl()
        {
            return new BooleanSlotControlView(
                this,
                _labels,
                _get, 
                _set);
        }
    }
}