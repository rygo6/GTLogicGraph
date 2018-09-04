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