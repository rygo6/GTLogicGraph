using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    /// <summary>
    /// Describes how to draw a node, paired with GenericNodeview
    /// </summar y>
    [Serializable]
    public abstract class LogicNodeEditor
    {
        [NonSerialized] private List<PortDescription> _portDescriptions = new List<PortDescription>();

        [SerializeField] private string _displayName;
        
        [SerializeField] private Vector3 _position;

        [SerializeField] private bool _expanded = true;

        [SerializeField] private string _nodeGuid;

        public LogicGraphView Owner { get; set; }
        public SerializedNode SerializedNode { get; set; }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public bool Expanded
        {
            get { return _expanded; }
            set { _expanded = value; }
        }

        public string NodeGuid
        {
            get { return _nodeGuid; }
        }

        public LogicNodeEditor()
        {
            _nodeGuid = System.Guid.NewGuid().ToString();
            ConstructNode();
        }

        public abstract void ConstructNode();

        public string NodeType()
        {
            var attrs = GetType().GetCustomAttributes(typeof(NodeEditorType), false) as NodeEditorType[];
            if (attrs != null && attrs.Length > 0)
            {
                return attrs[0].NodeType.Name;
            }
            else
            {
                Debug.LogWarning(this.GetType() + " requires a NodeType attribute");
                return "";
            }
        }

        public void GetInputSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isInputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetOutputSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isOutputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetSlots<T>(List<T> foundSlots) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void SetDirty()
        {
            SerializedNode.JSON = JsonUtility.ToJson(this);
        }

        public void AddPort(PortDescription portDescription)
        {
            if (!(portDescription is PortDescription))
                throw new ArgumentException(string.Format(
                    "Trying to add slot {0} to Material node {1}, but it is not a {2}", portDescription, this,
                    typeof(PortDescription)));

            _portDescriptions.Add(portDescription);
        }

        public T FindPort<T>(string memberName) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.MemberName == memberName && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        public T FindInputPort<T>(string memberName) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isInputSlot && slot.MemberName == memberName && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        public T FindOutputPort<T>(string memberName) where T : PortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isOutputSlot && slot.MemberName == memberName && slot is T)
                    return (T) slot;
            }

            return default(T);
        }
    }
}