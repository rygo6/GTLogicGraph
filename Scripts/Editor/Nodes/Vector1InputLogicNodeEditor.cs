using GeoTetra.GTLogicGraph.Ports;

namespace GeoTetra.GTLogicGraph
{
    [Title("Input", "Vector1")]
    [NodeEditorType(typeof(Vector1InputLogicNode))]
    public class Vector1InputLogicNodeEditor : AbstractLogicNodeEditor, IInputNode
    {        
        public override void ConstructNode()
        {
            AddSlot(new Vector1LogicSlot(this, "Vector1Output", "Out", SlotDirection.Output));
        }
    }
}

