using System;
using System.Security.AccessControl;
using System.Text;
using UnityEditor.Graphing;
using UnityEngine;
using UnityEngine.UI;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public class Vector1GenericProperty : AbstractGenericProperty<float>
    {
        public Vector1GenericProperty()
        {
            displayName = "Vector1";
        }

        public override GenericPropertyType propertyType
        {
            get { return GenericPropertyType.Vector1; }
        }

        public override Vector4 defaultValue
        {
            get { return new Vector4(value, value, value, value); }
        }

        public override Type GetPropertyType()
        {
            return typeof(float);
        }

        public override INode ToConcreteNode()
        {
            return  new ExampleGenericNode();
        }
    }
}
