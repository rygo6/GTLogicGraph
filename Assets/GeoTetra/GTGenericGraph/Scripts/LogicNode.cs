using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public abstract class LogicNode
    {
        public event Action<LogicNode> Changed;
        
        [SerializeField] 
        private Vector3 _position;
        
        [SerializeField] 
        private bool _expanded = true;

//        public Vector3 Position
//        {
//            get { return _position; }
//            set { _position = value; }
//        }
//
//        public bool Expanded
//        {
//            get { return _expanded; }
//            set { _expanded = value; }
//        }

        public void OnBeforeSerialize()
        {
//            Debug.Log("OnBeforeSerialize");
        }

        public void OnAfterDeserialize()
        {
//            Debug.Log("OnAfterDeserialize");
            if (Changed != null) Changed(this);
        }

        public virtual Action<float> InputSlot(int id)
        {
            return null;
        }

        public virtual Action<float> OutputSlot(int id)
        {
            return  null;
        }
    }
}