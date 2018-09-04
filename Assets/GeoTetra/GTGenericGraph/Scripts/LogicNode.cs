using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public class LogicNode
    {        
        [SerializeField] 
        private string _nodeGuid;

        public string NodeGuid
        {
            get { return _nodeGuid; }
        }
    }
}