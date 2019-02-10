using GeoTetra.GTLogicGraph.Ports;

namespace GeoTetra.GTLogicGraph
{
    [Title("Input", "Vector1")]
    [NodeEditorType(typeof(Vector1InputLogicNode))]
    public class Vector1InputLogicNodeEditor : LogicNodeEditor, IInputNode
    {        
        public override void ConstructNode()
        {
            AddPort(new Vector1PortDescription(this, "Vector1Output", "Out", PortDirection.Output));
        }
    }
}

