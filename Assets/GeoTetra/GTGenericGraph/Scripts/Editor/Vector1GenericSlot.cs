using System;
using System.Collections.Generic;
using GeoTetra.GTGenericGraph;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph.Drawing.Slots;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace UnityEditor.ShaderGraph
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
            string shaderOutputName,
            SlotType slotType,
            float value,
            ShaderStage shaderStage = ShaderStage.Dynamic,
            string label1 = "X",
            bool hidden = false)
            : base(slotId, displayName, shaderOutputName, slotType, hidden)
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
            return new MultiFloatSlotControlView(owner, _labels, () => new Vector4(value, 0f, 0f, 0f), (newValue) => value = newValue.x);
        }

        protected override string ConcreteSlotValueAsVariable(AbstractMaterialNode.OutputPrecision precision)
        {
            return NodeUtils.FloatToShaderValue(value);
        }

        public override void AddDefaultProperty(PropertyCollector properties, GenerationMode generationMode)
        {
            if (!generationMode.IsPreview())
                return;

            var matOwner = owner as AbstractGenericNode;
            if (matOwner == null)
                throw new Exception(string.Format("Slot {0} either has no owner, or the owner is not a {1}", this, typeof(AbstractMaterialNode)));

            var property = new Vector1ShaderProperty()
            {
                overrideReferenceName = matOwner.GetVariableNameForSlot(id),
                generatePropertyBlock = false,
                value = value
            };
            properties.AddShaderProperty(property);
        }

        public override SlotValueType valueType { get { return SlotValueType.Vector1; } }
        public override ConcreteSlotValueType concreteValueType { get { return ConcreteSlotValueType.Vector1; } }

        public override void GetPreviewProperties(List<PreviewProperty> properties, string name)
        {
            var pp = new PreviewProperty(PropertyType.Vector1)
            {
                name = name,
                floatValue = value,
            };
            properties.Add(pp);
        }

        public override void CopyValuesFrom(GenericSlot foundSlot)
        {
            var slot = foundSlot as Vector1GenericSlot;
            if (slot != null)
                value = slot.value;
        }
    }
}
