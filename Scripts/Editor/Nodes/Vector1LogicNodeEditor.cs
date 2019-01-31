using GeoTetra.GTLogicGraph.Slots;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Basic", "Vector1")]
    [NodeEditorType(typeof(Vector1LogicNode))]
    public class Vector1LogicNodeEditor : LogicNodeEditor
    {
        [SerializeField]
        private float _value;

        [SerializeField]
        private bool _boolValue;

        [NodeToggleControl("Bool")]
        public bool BoolValue
        {
            get { return _boolValue; }
            set
            {
                _boolValue = value;
                SetDirty();
            }
        }
        
        public override void ConstructNode()
        {
            AddPort(new Vector1PortDescription(this, "Vector1Input", "X", PortDirection.Input));
            AddPort(new Vector1PortDescription(this, "Vector1Output", "Out", PortDirection.Output));
//            AddSlot(new BooleanPortDescription(this, 3, InputPortDisplayName, PortDirection.Input));
//            AddSlot(new BooleanPortDescription(this, 4, OutputPortDisplayName, PortDirection.Output));
        }
    }
}