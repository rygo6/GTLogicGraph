using UnityEngine;
using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;

namespace GeoTetra.GTGenericGraph
{
    [Title("Output", "Vector1")]
    public class Vector1OutputNodeEditor : NodeEditor
    {
        private const string ValueName = "_value";
        private SerializedObject _serializedLogicNode;
        private SerializedProperty _serializedValue;
        private const string InputSlotXName = "X";
        private const int InputSlotId = 0;

        public override LogicNode SetLogicInstance()
        {
            throw new NotImplementedException();
        }

        public override LogicNode CreateLogicInstance()
        {
            TargetLogicNode = new Vector1LogicNode();
            _serializedLogicNode = new SerializedObject(TargetLogicNode);
            _serializedValue = _serializedLogicNode.FindProperty(ValueName);
            return TargetLogicNode;
        }

        public override void ConstructNode()
        {
            AddSlot(new Vector1GenericSlot(this, InputSlotId, InputSlotXName, SlotType.Input, null));
        }

        public override string DisplayName()
        {
            return "Vector 1 Output";
        }

    }
}

