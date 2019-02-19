using System;

namespace GeoTetra.GTLogicGraph.Ports
{
    [Serializable]
    public class Vector1LogicSlot : LogicSlot
    {
        public override SlotValueType ValueType { get { return SlotValueType.Vector1; } }

        public Vector1LogicSlot(AbstractLogicNodeEditor owner, string memberName, string displayName, SlotDirection direction) 
            : base(owner, memberName, displayName, direction)
        {
        }

        public override bool IsCompatibleWithInputSlotType(SlotValueType inputType)
        {
            return inputType == SlotValueType.Vector1;
        }
    }
}
