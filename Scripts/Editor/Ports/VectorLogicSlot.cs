using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTLogicGraph.Ports
{
    [Serializable]
    public class VectorLogicSlot : LogicSlot
    {        
        private readonly string[] _labels;
        private readonly Func<Vector4> _get;
        private readonly Action<Vector4> _set;
        
        public override SlotValueType ValueType
        {
            get { return SlotValueType.Vector3; }
        }

        public VectorLogicSlot(
            AbstractLogicNodeEditor owner, 
            string memberName, 
            string displayName, 
            SlotDirection direction,
            string[] labels,
            Func<Vector4> get, 
            Action<Vector4> set) 
            : base(owner, memberName, displayName, direction)
        {
            _get = get;
            _set = set;
            _labels = labels;
        }

        public override bool IsCompatibleWithInputSlotType(SlotValueType inputType)
        {
            return inputType == SlotValueType.Vector3;
        }
        
        public override VisualElement InstantiateControl()
        {
            return new MultiFloatSlotControlView(
                Owner,
                _labels,
                _get, 
                _set);
        }
    }
}
