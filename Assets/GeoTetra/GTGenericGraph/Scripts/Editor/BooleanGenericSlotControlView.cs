using System;
using UnityEditor.Graphing;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
	public class BooleanGenericSlotControlView : VisualElement
	{
		BooleanGenericPortDescription _portDescription;

		public BooleanGenericSlotControlView(BooleanGenericPortDescription portDescription)
		{
			AddStyleSheetPath("Styles/Controls/BooleanSlotControlView");
			_portDescription = portDescription;
			Action changedToggle = () => { OnChangeToggle(); };
			var toggleField = new UnityEngine.Experimental.UIElements.Toggle(changedToggle);
			Add(toggleField);
		}

		void OnChangeToggle()
		{
			_portDescription.Owner.Owner.GraphData.RegisterCompleteObjectUndo("Toggle Change");
//			var value = _slot.value;
//			value = !value;
//			_slot.value = value;
			_portDescription.Owner.SetDirty();
		}
	}
}