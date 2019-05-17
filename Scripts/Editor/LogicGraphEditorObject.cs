using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
	/// <summary>
	/// ScriptableObject to hold LogicGraphData to recieve serialize callbacks and undo.
	/// </summary>
	public class LogicGraphEditorObject : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField]
		private LogicGraphData _logicGraphData;
		
		public event Action Deserialized;
		
		public LogicGraphData LogicGraphData
		{
			get { return _logicGraphData; }
		}

		public void Initialize(LogicGraphData logicGraphData)
		{
			_logicGraphData = logicGraphData;
			if (_logicGraphData == null)
			{
				_logicGraphData = new LogicGraphData();
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
//			Debug.Log("OnBeforeSerialize");
		}

		public void OnAfterDeserialize()
		{
//			Debug.Log("OnAfterDeserialize");
			if (Deserialized != null) Deserialized();
		}
	}
}
