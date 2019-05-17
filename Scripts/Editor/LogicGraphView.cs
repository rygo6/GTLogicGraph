using System.Collections;
using System.Collections.Generic;
using GeoTetra.GTLogicGraph.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
	/// <summary>
	/// Implementation of GraphView
	/// </summary>
	public class LogicGraphView : GraphView
	{		
		public LogicGraphEditorObject LogicGraphEditorObject { get; private set; }
		
		public LogicGraphView()
		{
			this.LoadAndAddStyleSheet("Styles/LogicGraphView");
			Debug.Log("LogicGraphView Constructor");
		}
		
		public LogicGraphView(LogicGraphEditorObject logicGraphEditorObject) : this()
		{
			LogicGraphEditorObject = logicGraphEditorObject;
		}
		
		public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
		{
			var compatibleAnchors = new List<Port>();
			var startSlot = (startAnchor as LogicPort).Slot;
			if (startSlot == null)
				return compatibleAnchors;

			foreach (var candidateAnchor in ports.ToList())
			{
				var candidateSlot = (candidateAnchor as LogicPort).Slot;
				if (!startSlot.IsCompatibleWith(candidateSlot))
					continue;

				compatibleAnchors.Add(candidateAnchor);
			}
			return compatibleAnchors;
		}
	}
}