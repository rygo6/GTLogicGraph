using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public abstract class LogicNode : ScriptableObject, ISerializationCallbackReceiver
    {
        public event Action<LogicNode> Changed;
        
        [SerializeField] 
        private Vector3 _position;
        
        [SerializeField] 
        private bool _expanded = true;

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public bool Expanded
        {
            get { return _expanded; }
            set { _expanded = value; }
        }

        public void OnBeforeSerialize()
        {
//            Debug.Log("OnBeforeSerialize");
        }

        public void OnAfterDeserialize()
        {
//            Debug.Log("OnAfterDeserialize");
            if (Changed != null) Changed(this);
        }

        public abstract Action<float> InputSlot(int id);
        
        public abstract Action<float> OutputSlot(int id);
    }
}