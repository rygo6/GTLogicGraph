using GeoTetra.GTLogicGraph.Ports;

namespace GeoTetra.GTLogicGraph
{
    [Title("Output", "Vector1")]
    [NodeEditorType(typeof(Vector1OutputLogicNode))]
    public class Vector1OutputLogicNodeEditor : LogicNodeEditor, IOutputNode
    {
        public override void ConstructNode()
        {
            AddPort(new Vector1PortDescription(this, "Vector1Input", "X", PortDirection.Input));
        }
    }
}

