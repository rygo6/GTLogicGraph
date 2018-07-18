using System;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public abstract class AbstractGenericProperty<T> : IGenericProperty
    {
        [SerializeField]
        private T m_Value;

        [SerializeField]
        private string m_Name;

        [SerializeField]
        private SerializableGuid m_Guid = new SerializableGuid();

        public T value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public string displayName
        {
            get
            {
                if (string.IsNullOrEmpty(m_Name))
                    return guid.ToString();
                return m_Name;
            }
            set { m_Name = value; }
        }

        public abstract GenericPropertyType propertyType { get; }

        public Guid guid
        {
            get { return m_Guid.guid; }
        }

        public abstract Vector4 defaultValue { get; }
        public abstract Type GetPropertyType();

        public abstract INode ToConcreteNode();
    }
}
