using System;

namespace GeoTetra.GTLogicGraph.Slots
{
    [Serializable]
    public class Vector3PortDescription : PortDescription
    {
        public override PortValueType ValueType
        {
            get { return PortValueType.Vector3; }
        }

        public Vector3PortDescription(NodeEditor owner, string memberName, string displayName, PortDirection portDirection) 
            : base(owner, memberName, displayName, portDirection)
        {
        }

        public override bool IsCompatibleWithInputSlotType(PortValueType inputType)
        {
            return inputType == PortValueType.Vector3;
        }
    }
}
