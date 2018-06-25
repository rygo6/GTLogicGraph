using System;
using UnityEditor.Graphing;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
	public class BooleanGenericSlotControlView : VisualElement
	{
		BooleanGenericSlot _slot;

		public BooleanGenericSlotControlView(BooleanGenericSlot slot)
		{
			AddStyleSheetPath("Styles/Controls/BooleanSlotControlView");
			_slot = slot;
			Action changedToggle = () => { OnChangeToggle(); };
			var toggleField = new UnityEngine.Experimental.UIElements.Toggle(changedToggle);
			Add(toggleField);
		}

		void OnChangeToggle()
		{
			_slot.owner.owner.owner.RegisterCompleteObjectUndo("Toggle Change");
			var value = _slot.value;
			value = !value;
			_slot.value = value;
			_slot.owner.Dirty(ModificationScope.Node);
		}
	}
}