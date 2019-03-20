using System;
using GeoTetra.GTCommon;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTLogicGraph
{
    public class BooleanSlotControlView : VisualElement
    {
        private readonly BooleanSlot _slot;
        private readonly Func<Bool4> _get;
        private readonly Action<Bool4> _set;
        
        public BooleanSlotControlView(BooleanSlot slot, string[] labels, Func<Bool4> get, Action<Bool4> set)
        {
            AddStyleSheetPath("Styles/Controls/BooleanSlotControlView");
            _slot = slot;
            _get = get;
            _set = set;

            for (var i = 0; i < labels.Length; i++)
            {
                var label = new Label(labels[i]);
                Add(label);
                
                var toggleField = new Toggle();
                toggleField.value = _get()[i];
                toggleField.OnValueChanged(evt =>
                {
                    _slot.Owner.Owner.LogicGraphEditorObject.RegisterCompleteObjectUndo("Toggle Change");
                    var value = _get();
                    value[i] = evt.newValue;
                    _set(value);
                    _slot.Owner.SetDirty();
                });
                Add(toggleField);
            }
        }
    }
}
