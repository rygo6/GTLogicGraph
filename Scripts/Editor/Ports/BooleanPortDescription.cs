using System;

namespace GeoTetra.GTLogicGraph
{
    [Serializable]
    public class BooleanPortDescription : PortDescription
    {
        public override PortValueType ValueType { get { return PortValueType.Boolean; } }

        public BooleanPortDescription(LogicNodeEditor owner, string memberName, string displayName,
            PortDirection portDirection) : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Boolean;
        }
    }
}