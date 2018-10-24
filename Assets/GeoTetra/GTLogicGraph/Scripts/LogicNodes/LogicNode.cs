using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    [Serializable]
    public class LogicNode
    {        
        [SerializeField]
        private string _displayName;
        
        [SerializeField]
        private string _nodeGuid;

        public string NodeGuid
        {
            get { return _nodeGuid; }
        }
        
        public string DisplayName
        {
            get { return _displayName; }
        }
    }
}