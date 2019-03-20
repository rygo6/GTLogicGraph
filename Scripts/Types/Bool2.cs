using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTCommon
{
    [System.Serializable]
    public struct Bool2
    {
        public bool X;
        public bool Y;

        public Bool2(bool x)
        {
            X = x;
            Y = false;
        }

        public Bool2(bool x, bool y)
        {
            X = x;
            Y = y;
        }
    }
}