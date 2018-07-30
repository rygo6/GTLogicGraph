using System;
using GeoTetra.GTGenericGraph.Slots;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Title("Basic", "Vector1")]
    public class Vector1GenericNodeEditor : NodeEditor
    {
        private const string ValueName = "_value";
        private SerializedObject _serializedLogicNode;
        private SerializedProperty _serializedValue;
        
        const string kInputSlotXName = "X";
        const string kOutputSlotName = "Out";

        public const int InputSlotXId = 0;
        public const int OutputSlotId = 1;

        public override LogicNode SetLogicInstance()
        {
            throw new NotImplementedException();
        }

        public override LogicNode CreateLogicInstance()
        {
            TargetLogicNode = ScriptableObject.CreateInstance<Vector1LogicNode>();
            _serializedLogicNode = new SerializedObject(TargetLogicNode);
            _serializedValue = _serializedLogicNode.FindProperty(ValueName);
            ConstructNode();
            return TargetLogicNode;
        }

        public override string DisplayName()
        {
            return "Vector 1";
        }
        
        public override void ConstructNode()
        {
            AddSlot(new Vector1GenericSlot(InputSlotXId, kInputSlotXName, kInputSlotXName, SlotType.Input, _serializedValue.floatValue));
            AddSlot(new Vector1GenericSlot(OutputSlotId, kOutputSlotName, kOutputSlotName, SlotType.Output, 0));
        }
    }
}