using System;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph.Slots
{
    [Serializable]
    public class Vector1GenericSlot : GenericSlot, IGenericSlotHasValue<float>
    {
        [SerializeField]
        float _value;

        [SerializeField]
        float _defaultValue;

        string[] _labels;

        public Vector1GenericSlot()
        {}

        public Vector1GenericSlot(
            int slotId,
            string displayName,
            string outputName,
            SlotType slotType,
            float value,
            string label1 = "X",
            bool hidden = false)
            : base(slotId, displayName, slotType, hidden)
        {
            _defaultValue = value;
            _value = value;
            _labels = new[] { label1 };
        }

        public float defaultValue { get { return _defaultValue; } }

        public float value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override VisualElement InstantiateControl()
        {
            return new MultiFloatSlotControlView(
                Owner, 
                _labels, 
                () => new Vector4(value, 0f, 0f, 0f), 
                (newValue) => value = newValue.x);
        }

        protected override string ConcreteSlotValueAsVariable(AbstractMaterialNode.OutputPrecision precision)
        {
            return NodeUtils.FloatToShaderValue(value);
        }

        public override SlotValueType valueType { get { return SlotValueType.Vector1; } }
        public override ConcreteSlotValueType concreteValueType { get { return ConcreteSlotValueType.Vector1; } }

        public override void CopyValuesFrom(GenericSlot foundSlot)
        {
            var slot = foundSlot as Vector1GenericSlot;
            if (slot != null)
                value = slot.value;
        }
    }
}
