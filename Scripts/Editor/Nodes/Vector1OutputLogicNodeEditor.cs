using GeoTetra.GTLogicGraph.Ports;

namespace GeoTetra.GTLogicGraph
{
    [Title("Output", "Vector1")]
    [NodeEditorType(typeof(Vector1OutputLogicNode))]
    public class Vector1OutputLogicNodeEditor : AbstractLogicNodeEditor, IOutputNode
    {
        public override void ConstructNode()
        {
            AddSlot(new Vector1LogicSlot(this, "Vector1Input", "X", SlotDirection.Input));
        }
    }
}

