using System;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph.Slots
{
    [Serializable]
    public class Vector1GenericPortDescription : GenericPortDescription
    {
        public Vector1GenericPortDescription()
        {}

        public Vector1GenericPortDescription(
            NodeDescription owner,
            int slotId,
            string displayName,
            SlotType slotType)
            : base(owner, slotId, displayName, slotType)
        {

        }

        public override SlotValueType valueType { get { return SlotValueType.Vector1; } }
        public override ConcreteSlotValueType concreteValueType { get { return ConcreteSlotValueType.Vector1; } }
    }
}
