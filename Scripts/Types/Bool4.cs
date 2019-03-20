using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTCommon
{
    [System.Serializable]
    public struct Bool4
    {
        public bool X;
        public bool Y;
        public bool Z;
        public bool W;

        public Bool4(bool x)
        {
            X = x;
            Y = false;
            Z = false;
            W = false;
        }
        
        public Bool4(bool x, bool y)
        {
            X = x;
            Y = y;
            Z = false;
            W = false;
        }

        public Bool4(bool x, bool y, bool z)
        {
            X = x;
            Y = y;
            Z = z;
            W = false;
        }

        public Bool4(bool x, bool y, bool z, bool w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        
        public bool this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return X;
                    case 1:
                        return Y;
                    case 2:
                        return Z;
                    case 3:
                        return W;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4 index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        X = value;
                        break;
                    case 1:
                        Y = value;
                        break;
                    case 2:
                        Z = value;
                        break;
                    case 3:
                        W = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Vector4 index!");
                }
            }
        }
        
        public static implicit operator Bool4(Bool3 v)
        {
            return new Bool4(v.X, v.Y, v.Z, false);
        }

        public static implicit operator Bool3(Bool4 v)
        {
            return new Bool3(v.X, v.Y, v.Z);
        }

        public static implicit operator Bool4(Bool2 v)
        {
            return new Bool4(v.X, v.Y, false, false);
        }

        public static implicit operator Bool2(Bool4 v)
        {
            return new Bool2(v.X, v.Y);
        }
    }
}