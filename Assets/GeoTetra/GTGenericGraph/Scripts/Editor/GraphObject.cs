using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
	public class GraphObject : ScriptableObject, ISerializationCallbackReceiver
	{
		public event Action Deserialized;
		
		public GraphData GraphData { get; private set; }

		public void Initialize(GraphData graphData)
		{
			GraphData = graphData;
			if (GraphData == null)
			{
				GraphData = new GraphData();
			}
		}
		
		public void RegisterCompleteObjectUndo(string name)
		{
			Undo.RegisterCompleteObjectUndo(this, name);
		}
		
		public void OnBeforeSerialize()
		{
//			Debug.Log("OnBeforeSerialize");
		}

		public void OnAfterDeserialize()
		{
			Debug.Log("OnAfterDeserialize");
			if (Deserialized != null) Deserialized();
		}
	}
}
