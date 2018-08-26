using System;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph.Slots
{
    [Serializable]
    public class Vector1GenericSlot : GenericSlot
    {
        float _value;

        string[] _labels;

        public Vector1GenericSlot()
        {}

        public Vector1GenericSlot(
            NodeEditor owner,
            int slotId,
            string displayName,
            SlotType slotType,
            float value,
            string label1 = "X",
            bool hidden = false)
            : base(owner, slotId, displayName, slotType, hidden)
        {
            _value = value;
            _labels = new[] { label1 };
        }

        public override SlotValueType valueType { get { return SlotValueType.Vector1; } }
        public override ConcreteSlotValueType concreteValueType { get { return ConcreteSlotValueType.Vector1; } }

        public override void CopyValuesFrom(GenericSlot foundSlot)
        {
//            var slot = foundSlot as Vector1GenericSlot;
//            if (slot != null)
//                value = slot.value;
        }
    }
}
