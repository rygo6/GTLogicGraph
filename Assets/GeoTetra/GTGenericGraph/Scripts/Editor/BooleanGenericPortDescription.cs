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
    public class BooleanGenericPortDescription : GenericPortDescription
    {
        public BooleanGenericPortDescription()
        {}

        public BooleanGenericPortDescription(
            NodeDescription owner,
            int slotId,
            string displayName,
            SlotType slotType)
            : base(owner, slotId, displayName, slotType)
        {

        }

        public override SlotValueType valueType { get { return SlotValueType.Boolean; } }
        public override ConcreteSlotValueType concreteValueType { get { return ConcreteSlotValueType.Boolean; } }
    }
}