using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    public class Vector1LogicNode : LogicNode
    {
        [NodePort] public event Action<float> Vector1Output;

        private float _value;
        
        [NodePort]
        public void Vector1Input(float value)
        {
            Debug.Log("Vector1LogicNode SetValue " + value);
            _value = value;
            if (Vector1Output != null) Vector1Output(_value);
        }
    }
}