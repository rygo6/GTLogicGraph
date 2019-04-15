using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    public class Vector3InputLogicNode : LogicNode
    {
        public event Action<Vector3> Vector3Output;
       
        [Vector3Input]
        public void Vector3Input(Vector3 value)
        {
            Debug.Log("Vector1InputLogicNode Vector3Input " + value);
            if (Vector3Output != null) Vector3Output(value);
        }
    }
}