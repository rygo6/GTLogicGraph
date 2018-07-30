using System;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public class BooleanGenericSlot : GenericSlot, IMaterialSlotHasValue<bool>
    {
        [SerializeField]
        private bool m_Value;

        [SerializeField]
        private bool m_DefaultValue;

        public BooleanGenericSlot()
        {}

        public BooleanGenericSlot(
            int slotId,
            string displayName,
            SlotType slotType,
            bool value,
            bool hidden = false)
            : base(slotId, displayName, slotType, hidden)
        {
            m_DefaultValue = value;
            m_Value = value;
        }

        public override VisualElement InstantiateControl()
        {
            return new BooleanGenericSlotControlView(this);
        }

        public bool defaultValue { get { return m_DefaultValue; } }

        public bool value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        protected override string ConcreteSlotValueAsVariable(AbstractMaterialNode.OutputPrecision precision)
        {
            return (value ? 1 : 0).ToString();
        }

        public override SlotValueType valueType { get { return SlotValueType.Boolean; } }
        public override ConcreteSlotValueType concreteValueType { get { return ConcreteSlotValueType.Boolean; } }

        public override void CopyValuesFrom(GenericSlot foundSlot)
        {
            var slot = foundSlot as BooleanGenericSlot;
            if (slot != null)
                value = slot.value;
        }
    }
}