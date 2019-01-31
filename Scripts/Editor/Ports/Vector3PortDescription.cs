using System;
using UnityEditor.ShaderGraph.Drawing.Slots;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTLogicGraph.Slots
{
    [Serializable]
    public class Vector3PortDescription : PortDescription
    {
        public override PortValueType ValueType
        {
            get { return PortValueType.Vector3; }
        }

        public Vector3PortDescription(LogicNodeEditor owner, string memberName, string displayName, PortDirection portDirection) 
            : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Vector3;
        }
        
//        public override VisualElement InstantiateControl()
//        {
//            return new MultiFloatSlotControlView(owner, m_Labels, () => new Vector4(value, 0f, 0f, 0f), (newValue) => value = newValue.x);
//        }
    }
}
