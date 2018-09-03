using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
	public class GenericGraphView : GraphView
	{		
		public GraphObject GraphObject { get; private set; }
		
		public GenericGraphView()
		{
			AddStyleSheetPath("Styles/GenericGraphView");
			Debug.Log("GenericGraphView Constructor");
		}
		
		public GenericGraphView(GraphObject graphObject) : this()
		{
			GraphObject = graphObject;
		}
		
		public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
		{
			var compatibleAnchors = new List<Port>();
			var startSlot = (startAnchor as PortView).PortDescription;
			if (startSlot == null)
				return compatibleAnchors;

			foreach (var candidateAnchor in ports.ToList())
			{
				var candidateSlot = (candidateAnchor as PortView).PortDescription;
				if (!startSlot.IsCompatibleWith(candidateSlot))
					continue;

				compatibleAnchors.Add(candidateAnchor);
			}
			return compatibleAnchors;
		}
	}
}