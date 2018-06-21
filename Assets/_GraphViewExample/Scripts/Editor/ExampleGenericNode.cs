using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace GeoTetra.GenericGraph
{
    public class ExampleGenericNode : AbstractGenericNode
    {
        [SerializeField]
        private bool m_Value;

        public const int OutputSlotId = 0;
        private const string kOutputSlotName = "Out";

        public ExampleGenericNode()
        {
            name = "Boolean";
            UpdateNodeAfterDeserialization();
        }

        public override string DocumentationUrl
        {
            get { return "https://github.com/Unity-Technologies/ShaderGraph/wiki/Boolean-Node"; }
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
//            AddSlot(new BooleanMaterialSlot(OutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output, false));
//            RemoveSlotsNameNotMatching(new[] {OutputSlotId});
        }

        public ToggleData Value
        {
            get { return new ToggleData(m_Value); }
            set
            {
                if (m_Value == value.isOn)
                    return;
                m_Value = value.isOn;
                Dirty(ModificationScope.Node);
            }
        }

//        public override void CollectShaderProperties(PropertyCollector properties, GenerationMode generationMode)
//        {
//            if (!generationMode.IsPreview())
//                return;
//
//            properties.AddShaderProperty(new BooleanShaderProperty()
//            {
//                overrideReferenceName = GetVariableNameForNode(),
//                generatePropertyBlock = false,
//                value = m_Value
//            });
//        }

        public override string GetVariableNameForSlot(int slotId)
        {
            return GetVariableNameForNode();
        }

        public override void CollectPreviewMaterialProperties(List<PreviewProperty> properties)
        {
            properties.Add(new PreviewProperty(PropertyType.Boolean)
            {
                name = GetVariableNameForNode(),
                booleanValue = m_Value
            });
        }

        public int outputSlotId
        {
            get { return OutputSlotId; }
        }
    }
}