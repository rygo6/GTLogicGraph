using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
	public class GenericGraphView : GraphView
	{		
		public GenericGraph Graph { get; private set; }
		
		public GenericGraphView()
		{
			AddStyleSheetPath("Styles/GenericGraphView");
			Debug.Log("GenericGraphView Constructor");
		}
		
		public GenericGraphView(GenericGraph graph) : this()
		{
			Graph = graph;
		}
		
		public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
		{
			var compatibleAnchors = new List<Port>();
			var startSlot = startAnchor.GetSlot();
			if (startSlot == null)
				return compatibleAnchors;

			foreach (var candidateAnchor in ports.ToList())
			{
				var candidateSlot = candidateAnchor.GetSlot();
				if (!startSlot.IsCompatibleWith(candidateSlot))
					continue;

				compatibleAnchors.Add(candidateAnchor);
			}
			return compatibleAnchors;
		}
	}
}