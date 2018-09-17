using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    public class Vector1InputLogicNode : LogicNode
    {
        [NodePort]
        public event Action<float> Vector1Output;
       
        [FloatInput]
        public void Vector1Input(float value)
        {
            Debug.Log("Vector1InputLogicNode Vector1Input " + value);
            if (Vector1Output != null) Vector1Output(value);
        }
    }
}