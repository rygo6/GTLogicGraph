using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
	public class GraphObject : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField]
		private GraphData _graphData;
		
		public event Action Deserialized;
		
		public GraphData GraphData
		{
			get { return _graphData; }
		}

		public void Initialize(GraphData graphData)
		{
			_graphData = graphData;
			if (_graphData == null)
			{
				_graphData = new GraphData();
			}
		}
		
		public void RegisterCompleteObjectUndo(string name)
		{
#if UNITY_EDITOR
			UnityEditor.Undo.RegisterCompleteObjectUndo(this, name);
#endif
		}
		
		public void OnBeforeSerialize()
		{
			Debug.Log("OnBeforeSerialize");
		}

		public void OnAfterDeserialize()
		{
			Debug.Log("OnAfterDeserialize");
			if (Deserialized != null) Deserialized();
		}
	}
}
