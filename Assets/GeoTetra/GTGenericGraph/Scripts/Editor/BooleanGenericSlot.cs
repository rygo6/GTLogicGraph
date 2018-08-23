using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public class BooleanGenericSlot : GenericSlot
    {
        SerializedProperty _serializedValue;

        public BooleanGenericSlot()
        {}

        public BooleanGenericSlot(
            NodeEditor owner,
            int slotId,
            string displayName,
            SlotType slotType,
            SerializedProperty serializedValue,
            bool hidden = false)
            : base(owner, slotId, displayName, slotType, hidden)
        {
            _serializedValue = serializedValue;
        }

        public override SlotValueType valueType { get { return SlotValueType.Boolean; } }
        public override ConcreteSlotValueType concreteValueType { get { return ConcreteSlotValueType.Boolean; } }

        public override void CopyValuesFrom(GenericSlot foundSlot)
        {
//            var slot = foundSlot as BooleanGenericSlot;
//            if (slot != null)
//                value = slot.value;
        }
    }
}