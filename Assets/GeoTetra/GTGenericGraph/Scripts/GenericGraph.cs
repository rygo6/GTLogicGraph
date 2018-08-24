using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
	[CreateAssetMenu( fileName = "GenericGraphLogic", menuName = "Generic Graph Logic", order = 0)]
	public class GenericGraph : ScriptableObject
	{
		[SerializeField]
		private List<LogicNode> _logicNodes = new List<LogicNode>();
		
		[SerializeField]
		private List<GraphEdge> _edges = new List<GraphEdge>();

		public void RegisterCompleteObjectUndo(string name)
		{
			Undo.RegisterCompleteObjectUndo(this, name);
		}
		
		public void AddNode(LogicNode node)
		{
			Debug.Log("adding node" + node);
			_logicNodes.Add(node);
			Debug.Log(_logicNodes.Count);
		}

		public void AddEdge(GraphEdge edge)
		{
			Debug.Log("adding edge");
			_edges.Add(edge);
		}
	}

	[Serializable]
	public class GraphEdge
	{
		public LogicNode Source;
		public int SourceIndex;
		public LogicNode Target;
		public int TargetIndex;
	}
}