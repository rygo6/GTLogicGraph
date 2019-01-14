using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    public class Vector1OutputLogicNode : LogicNode
    {
        [Output]
        public event Action<float> Vector1Output;

        [SerializeField]
        private float _value;

        [LogicNodePort]
        public void Vector1Input(float value)
        {
            Debug.Log("Vector1OutputLogicNode SetValue " + value);
            _value = value;
            if (Vector1Output != null) Vector1Output(_value);
        }
    }
}