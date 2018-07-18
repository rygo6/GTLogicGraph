using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphing;
using UnityEditor.Graphing.Util;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Title("Output", "Property")]
    public class OutputNode : AbstractGenericNode, IGeneratesGraphLogic
    {
        [SerializeField]
        float m_Value;

        const int InputSlotId = 0;
        
        public OutputNode()
        {
            name = "Output";
            UpdateNodeAfterDeserialization();
        }
        
        public GraphLogicGenerator GetNodeChain()
        {
            return null;
        }

        public override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
        }

        public override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
        }

        public sealed override void UpdateNodeAfterDeserialization()
        {
            AddSlot(new Vector1GenericSlot(InputSlotId, "testBool", "tesBoolIn", SlotType.Input, m_Value));

            RemoveSlotsNameNotMatching(new[] {InputSlotId});
        }

        public void GenerateNodeLogic(GraphLogicGenerator visitor)
        {
            throw new NotImplementedException();
        }
    }
}

