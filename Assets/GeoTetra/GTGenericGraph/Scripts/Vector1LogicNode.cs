using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public class Vector1LogicNode : LogicNode
    {
        public event Action<float> output;

        [SerializeField] 
        private float _value;

        public float Value
        {
            get { return _value; }
        }

        public void SetValue(float value)
        {
            _value = value;
            if (output != null) output(_value);
        }

        public Action<float> InputSlot(int id)
        {
            switch (id)
            {
                case 0:
                    return SetValue;
            }

            return null;
        }
        
        public Action<float> OutputSlot(int id)
        {
            switch (id)
            {
                case 1:
                    return output;
            }

            return null;
        }
    }
}