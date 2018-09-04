using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [LogicNodeType("Vector1Input")]
    public class Vector1InputLogicNode : LogicNode
    {
        [PortIndex(0)]
        public event Action<float> output;
       
        [InputAttribute]
        public void Vector1Input(float value)
        {
            Debug.Log("Set Value " + value);
            if (output != null) output(value);
        }
    }
}