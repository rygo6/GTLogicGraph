using GeoTetra.GTLogicGraph.Ports;

namespace GeoTetra.GTLogicGraph
{
    [Title("Input", "Vector3")]
    [NodeEditorType(typeof(Vector3InputLogicNode))]
    public class Vector3InputLogicNodeEditor : LogicNodeEditor, IInputNode
    {
        private static readonly string[] Labels = {"X", "Y", "Z"};

        public override void ConstructNode()
        {
            AddPort(new VectorPortDescription(this,
                "Vector3Output",
                "Out",
                PortDirection.Output,
                Labels,
                null,
                null));
        }
    }
}