using GeoTetra.GTLogicGraph.Ports;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Title("Basic", "Vector1")]
    [NodeEditorType(typeof(Vector1LogicNode))]
    public class Vector1LogicNodeEditor : AbstractLogicNodeEditor
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
            AddSlot(new Vector1LogicSlot(this, "Vector1Input", "X", SlotDirection.Input));
            AddSlot(new Vector1LogicSlot(this, "Vector1Output", "Out", SlotDirection.Output));
//            AddSlot(new BooleanPortDescription(this, 3, InputPortDisplayName, PortDirection.Input));
//            AddSlot(new BooleanPortDescription(this, 4, OutputPortDisplayName, PortDirection.Output));
        }
    }
}