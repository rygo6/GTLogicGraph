using System;

namespace GeoTetra.GTLogicGraph.Ports
{
    [Serializable]
    public class Vector1PortDescription : PortDescription
    {
        public override PortValueType ValueType { get { return PortValueType.Vector1; } }

        public Vector1PortDescription(LogicNodeEditor owner, string memberName, string displayName, PortDirection portDirection) 
            : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Vector1;
        }
    }
}
