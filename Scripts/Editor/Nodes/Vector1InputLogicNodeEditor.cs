using GeoTetra.GTLogicGraph.Ports;

namespace GeoTetra.GTLogicGraph
{
    [Title("Input", "Vector1")]
    [NodeEditorType(typeof(Vector1InputLogicNode))]
    public class Vector1InputLogicNodeEditor : AbstractLogicNodeEditor, IInputNode
    {        
        private static readonly string[] Labels = {"X"};
        
        public override void ConstructNode()
        {
            AddSlot(new VectorLogicSlot(
                this,
                nameof(Vector1InputLogicNode.Vector1Output),
                "Vector1Output",
                SlotDirection.Output,
                Labels,
                null,
                null));
        }
    }
}

