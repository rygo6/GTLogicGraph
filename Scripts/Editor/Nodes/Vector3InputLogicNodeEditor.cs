using GeoTetra.GTLogicGraph.Ports;

namespace GeoTetra.GTLogicGraph
{
    [Title("Input", "Vector3")]
    [NodeEditorType(typeof(Vector3InputLogicNode))]
    public class Vector3InputLogicNodeEditor : AbstractLogicNodeEditor, IInputNode
    {
        private static readonly string[] Labels = {"X", "Y", "Z"};

        public override void ConstructNode()
        {
            AddSlot(new VectorLogicSlot(this,
                "Vector3Output",
                "Out",
                SlotDirection.Output,
                Labels,
                null,
                null));
        }
    }
}