using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [LogicNodeType("Vector1Output")]
    public class Vector1OutputLogicNode : LogicNode
    {
        [Output(1)]
        public event Action<float> output;

        [SerializeField] 
        private float _value;

        public float Value
        {
            get { return _value; }
        }

        [PortIndex(0)]
        public void SetValue(float value)
        {
            Debug.Log("Vector1OutputLogicNode SetValue " + value);
            _value = value;
            if (output != null) output(_value);
        }
    }
}