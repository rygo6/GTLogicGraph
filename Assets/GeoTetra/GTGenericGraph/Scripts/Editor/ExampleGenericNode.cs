using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Title("Example", "ExampleGenericNode")]
    public class ExampleGenericNode : AbstractGenericNode
    {
        [SerializeField]
        private float m_Value;

        const int InputSlotId = 0;
        const int OutputSlotId = 1;

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
            AddSlot(new Vector1GenericSlot(InputSlotId, "testBool", "tesBoolIn", SlotType.Input, m_Value));
            AddSlot(new Vector1GenericSlot(OutputSlotId, "testBool4", "tesBoolOut2", SlotType.Output, 0));

            RemoveSlotsNameNotMatching(new[] {OutputSlotId, InputSlotId});
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
    }
}