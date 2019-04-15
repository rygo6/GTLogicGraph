using GeoTetra.GTLogicGraph.Ports;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Output", "Vector1")]
    [NodeEditorType(typeof(Vector1OutputLogicNode))]
    public class Vector1OutputLogicNodeEditor : AbstractLogicNodeEditor, IOutputNode
    {
        [SerializeField] private float _value;

        private static readonly string[] Labels = {"X"};

        public override void ConstructNode()
        {
            AddSlot(new VectorLogicSlot(
                this,
                nameof(Vector1OutputLogicNode.Vector1Input),
                "Vector1Input",
                SlotDirection.Input,
                Labels,
                () => new Vector4(_value, 0, 0, 0),
                (newValue) => _value = newValue.x));
        }
    }
}