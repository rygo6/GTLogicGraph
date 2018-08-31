using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
	[Serializable]
	public class GraphData 
	{		
		[SerializeField]
		private List<SerializedNode> _serializedNodes = new List<SerializedNode>();
		
		[SerializeField]
		private List<SerializedEdge> _serializedEdges = new List<SerializedEdge>();
		
		public List<SerializedEdge> SerializedEdges
		{
			get { return _serializedEdges; }
		}

		public List<SerializedNode> SerializedNodes
		{
			get { return _serializedNodes; }
		}

		public void AddNode(SerializedNode node)
		{
			Debug.Log("adding node" + node);
			_serializedNodes.Add(node);
		}

		public void RemoveNode(SerializedNode node)
		{
			Debug.Log("removing node" + node);
			_serializedNodes.Remove(node);
		}
		
		public void AddEdge(SerializedEdge edge)
		{
			Debug.Log("adding edge");
			_serializedEdges.Add(edge);
		}
		
		public void RemoveEdge(SerializedEdge edge)
		{
			Debug.Log("removing edge");
			_serializedEdges.Remove(edge);
		}
	}

	[Serializable]
	public class SerializedNode
	{
		public string NodeType;
		public string NodeGuid;
		public string JSON;
	}
	
	[Serializable]
	public class SerializedEdge
	{
		public string Source;
		public int SourceIndex;
		public string Target;
		public int TargetIndex;
	}
}