using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTLogicGraph.Ports
{
    [Serializable]
    public class Vector3PortDescription : PortDescription
    {
        static readonly string[] k_Labels = {"X", "Y", "Z", "W"};
        
        public float value { get; set; }

        private Func<Vector4> _get;
        private Action<Vector4> _set;
        
        public override PortValueType ValueType
        {
            get { return PortValueType.Vector3; }
        }

        public Vector3PortDescription(
            LogicNodeEditor owner, 
            string memberName, 
            string displayName, 
            PortDirection portDirection,
            Func<Vector4> get, 
            Action<Vector4> set) 
            : base(owner, memberName, displayName, portDirection)
        {
            _get = get;
            _set = set;
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Vector3;
        }
        
        public override VisualElement InstantiateControl()
        {
            var labels = k_Labels.Take(3).ToArray();
            return new MultiFloatPortInputView(
                Owner, 
                labels, 
                _get, 
                _set);
        }
    }
}
