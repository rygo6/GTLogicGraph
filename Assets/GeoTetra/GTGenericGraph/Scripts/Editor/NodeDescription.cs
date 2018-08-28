using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    /// <summary>
    /// Describes how to draw a node, paired with GenericNodeview
    /// </summary>
    [Serializable]
    public abstract class NodeDescription
    {
        [NonSerialized] private List<GenericPortDescription> _portDescriptions = new List<GenericPortDescription>();

        [SerializeField] private Vector3 _position;

        [SerializeField] private bool _expanded = true;

        [SerializeField] private string _nodeGuid;

        public GenericGraphView Owner { get; set; }
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

        public NodeDescription()
        {
            _nodeGuid = System.Guid.NewGuid().ToString();
            ConstructNode();
        }

        public abstract void ConstructNode();
        public abstract string NodeType();

        public void GetInputSlots<T>(List<T> foundSlots) where T : GenericPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isInputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetOutputSlots<T>(List<T> foundSlots) where T : GenericPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isOutputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetSlots<T>(List<T> foundSlots) where T : GenericPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void SetDirty()
        {
            EditorUtility.SetDirty(Owner.GraphData);
        }

        public void AddSlot(GenericPortDescription portDescription)
        {
            if (!(portDescription is GenericPortDescription))
                throw new ArgumentException(string.Format(
                    "Trying to add slot {0} to Material node {1}, but it is not a {2}", portDescription, this,
                    typeof(GenericPortDescription)));

            _portDescriptions.Add(portDescription);
        }

        public T FindSlot<T>(int slotId) where T : GenericPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.id == slotId && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        public T FindInputSlot<T>(int slotId) where T : GenericPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isInputSlot && slot.id == slotId && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        public T FindOutputSlot<T>(int slotId) where T : GenericPortDescription
        {
            foreach (var slot in _portDescriptions)
            {
                if (slot.isOutputSlot && slot.id == slotId && slot is T)
                    return (T) slot;
            }

            return default(T);
        }
    }
}