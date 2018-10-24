using GeoTetra.GTLogicGraph.Slots;

namespace GeoTetra.GTLogicGraph
{
    [Title("Input", "Vector3")]
    [NodeEditorType(typeof(Vector3InputLogicNode))]
    public class Vector3InputNodeEditor : NodeEditor, IInputNode
    {        
        public override void ConstructNode()
        {
            AddSlot(new Vector3PortDescription(this, "Vector3Output", "Out", PortDirection.Output));
        }
    }
}

