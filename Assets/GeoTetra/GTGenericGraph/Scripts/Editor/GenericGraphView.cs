using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
	public class GenericGraphView : GraphView
	{
		public GenericGraphView()
		{
			AddStyleSheetPath("Styles/GenericGraphView");
			Debug.Log("GenericGraphView Constructor");
//			AddStyleSheetPath("Styles/MaterialGraphView");
//			serializeGraphElements = SerializeGraphElementsImplementation;
//			canPasteSerializedData = CanPasteSerializedDataImplementation;
//			unserializeAndPaste = UnserializeAndPasteImplementation;
//			deleteSelection = DeleteSelectionImplementation;
		}
	}
}