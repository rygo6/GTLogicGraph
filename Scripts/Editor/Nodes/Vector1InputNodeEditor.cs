using GeoTetra.GTLogicGraph.Slots;

namespace GeoTetra.GTLogicGraph
{
    [Title("Input", "Vector1")]
    [NodeEditorType(typeof(Vector1InputLogicNode))]
    public class Vector1InputNodeEditor : NodeEditor, IInputNode
    {        
        public override void ConstructNode()
        {
            AddSlot(new Vector1PortDescription(this, "Vector1Output", "Out", PortDirection.Output));
        }
    }
}

