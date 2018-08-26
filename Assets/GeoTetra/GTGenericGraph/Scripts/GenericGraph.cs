using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
	[CreateAssetMenu( fileName = "GenericGraphLogic", menuName = "Generic Graph Logic", order = 0)]
	public class GenericGraph : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField]
		private List<SerializedLogicNode> _serializedLogicNodes = new List<SerializedLogicNode>();
		
		[SerializeField]
		private List<SerializedEdge> _serializedEdges = new List<SerializedEdge>();

		public List<SerializedEdge> SerializedEdges
		{
			get { return _serializedEdges; }
		}

		public List<SerializedLogicNode> SerializedLogicNodes
		{
			get { return _serializedLogicNodes; }
		}

		public void RegisterCompleteObjectUndo(string name)
		{
			Undo.RegisterCompleteObjectUndo(this, name);
		}
		
		public void AddNode(SerializedLogicNode node)
		{
			Debug.Log("adding node" + node);
			SerializedLogicNodes.Add(node);
		}

		public void AddEdge(SerializedEdge edge)
		{
			Debug.Log("adding edge");
			_serializedEdges.Add(edge);
		}

		public void OnBeforeSerialize()
		{
//			Debug.Log("OnBeforeSerialize");
		}

		public void OnAfterDeserialize()
		{
//			Debug.Log("OnAfterDeserialize");
		}
	}

	[Serializable]
	public class SerializedLogicNode
	{
		public string NodeType;
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