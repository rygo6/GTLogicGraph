using GeoTetra.GTLogicGraph.Ports;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Basic", "Vector1")]
    [NodeEditorType(typeof(Vector1LogicNode))]
    public class Vector1LogicNodeEditor : AbstractLogicNodeEditor
    {
        [SerializeField]
        private float _value;

        [SerializeField]
        private bool _boolValue;

        private static readonly string[] Labels = {"X"};
        
        [NodeToggleControl("Bool")]
        public bool BoolValue
        {
            get { return _boolValue; }
            set
            {
                _boolValue = value;
                SetDirty();
            }
        }
        
        public override void ConstructNode()
        {
            AddSlot(new VectorLogicSlot(
                this,
                nameof(Vector1LogicNode.Vector1Input),
                "Vector1Input",
                SlotDirection.Input,
                Labels,
                () => new Vector4(_value, 0, 0, 0),
                (newValue) => _value = newValue.x));
            
            AddSlot(new VectorLogicSlot(
                this,
                nameof(Vector1LogicNode.Vector1Output),
                "Vector1Output",
                SlotDirection.Output,
                Labels,
                null,
                null));
        }
    }
}