using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Title("Example", "ExampleGenericNode")]
    public class ExampleGenericNode : AbstractGenericNode, IGeneratesGraphLogic
    {
        [SerializeField]
        float m_Value;

        const int InputSlotId = 0;
        const int OutputSlotId = 1;

        [SerializeField]
        bool m_ToggleTest;
        
        [GenericToggleControl("Red")]
        public GenericToggleData toggleTest
        {
            get { return new GenericToggleData(m_ToggleTest); }
            set
            {
                if (m_ToggleTest == value.isOn)
                    return;
                m_ToggleTest = value.isOn;
                Dirty(ModificationScope.Node);
            }
        }

        public ExampleGenericNode()
        {
            name = "Example";
            UpdateNodeAfterDeserialization();
        }

        public void GenerateNodeLogic(GraphLogicGenerator visitor)
        {

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

        public override string GetVariableNameForSlot(int slotId)
        {
            return GetVariableNameForNode();
        }
    }
}