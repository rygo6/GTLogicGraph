using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTCommon
{
    [System.Serializable]
    public struct Bool3
    {
        public bool X;
        public bool Y;
        public bool Z;

        public Bool3(bool x)
        {
            X = x;
            Y = false;
            Z = false;
        }

        public Bool3(bool x, bool y)
        {
            X = x;
            Y = y;
            Z = false;
        }

        public Bool3(bool x, bool y, bool z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}