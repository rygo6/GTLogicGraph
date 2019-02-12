using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTLogicGraph.Ports
{
    [Serializable]
    public class VectorPortDescription : PortDescription
    {        
        private string[] _labels;
        private Func<Vector4> _get;
        private Action<Vector4> _set;
        
        public override PortValueType ValueType
        {
            get { return PortValueType.Vector3; }
        }

        public VectorPortDescription(
            LogicNodeEditor owner, 
            string memberName, 
            string displayName, 
            PortDirection portDirection,
            string[] labels,
            Func<Vector4> get, 
            Action<Vector4> set) 
            : base(owner, memberName, displayName, portDirection)
        {
            _get = get;
            _set = set;
            _labels = labels;
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Vector3;
        }
        
        public override VisualElement InstantiateControl()
        {
            return new MultiFloatPortInputView(
                Owner,
                _labels,
                _get, 
                _set);
        }
    }
}
