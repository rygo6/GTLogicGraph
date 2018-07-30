using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public class LogicNode : ScriptableObject
    {
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
    }
}