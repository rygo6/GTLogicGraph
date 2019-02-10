using GeoTetra.GTLogicGraph.Ports;

namespace GeoTetra.GTLogicGraph
{
    [Title("Input", "Vector3")]
    [NodeEditorType(typeof(Vector3InputLogicNode))]
    public class Vector3InputLogicNodeEditor : LogicNodeEditor, IInputNode
    {        
        public override void ConstructNode()
        {
            AddPort(new Vector3PortDescription(this, "Vector3Output", "Out", PortDirection.Output, null, null));
        }
    }
}

