using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class NodeEditor
    {
        [NonSerialized]
        private List<GenericSlot> m_Slots = new List<GenericSlot>();

        public GenericGraphView Owner { get; set; }
        public LogicNode TargetLogicNode { get; protected set; }
        
        public abstract LogicNode SetLogicInstance();
        public abstract LogicNode CreateLogicInstance();
        public abstract void ConstructNode();
        public abstract string DisplayName();
        
        public void GetInputSlots<T>(List<T> foundSlots) where T : GenericSlot
        {
            foreach (var slot in m_Slots)
            {
                if (slot.isInputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetOutputSlots<T>(List<T> foundSlots) where T : GenericSlot
        {
            foreach (var slot in m_Slots)
            {
                if (slot.isOutputSlot && slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void GetSlots<T>(List<T> foundSlots) where T : GenericSlot
        {
            foreach (var slot in m_Slots)
            {
                if (slot is T)
                    foundSlots.Add((T) slot);
            }
        }

        public void SetDirty()
        {
            EditorUtility.SetDirty(TargetLogicNode);
        }

        public void AddSlot(GenericSlot slot)
        {
            if (!(slot is GenericSlot))
                throw new ArgumentException(string.Format(
                    "Trying to add slot {0} to Material node {1}, but it is not a {2}", slot, this,
                    typeof(GenericSlot)));

            var addingSlot = (GenericSlot) slot;
            var foundSlot = FindSlot<GenericSlot>(slot.id);

            // this will remove the old slot and add a new one
            // if an old one was found. This allows updating values
            m_Slots.RemoveAll(x => x.id == slot.id);
            m_Slots.Add(slot);

            if (foundSlot == null)
                return;

            addingSlot.CopyValuesFrom(foundSlot);
        }

        public void RemoveSlot(int slotId)
        {
            // Remove edges that use this slot
            // no owner can happen after creation
            // but before added to graph
//            if (owner != null)
//            {
//                var edges = owner.GetEdges(GetSlotReference(slotId));
//
//                foreach (var edge in edges.ToArray())
//                    owner.RemoveEdge(edge);
//            }
//
//            //remove slots
//            m_Slots.RemoveAll(x => x.id == slotId);
        }

        public void RemoveSlotsNameNotMatching(IEnumerable<int> slotIds, bool supressWarnings = false)
        {
            var invalidSlots = m_Slots.Select(x => x.id).Except(slotIds);

            foreach (var invalidSlot in invalidSlots.ToArray())
            {
                if (!supressWarnings)
                    Debug.LogWarningFormat("Removing Invalid MaterialSlot: {0}", invalidSlot);
                RemoveSlot(invalidSlot);
            }
        }
//
//        public SlotReference GetSlotReference(int slotId)
//        {
//            var slot = FindSlot<ISlot>(slotId);
//            if (slot == null)
//                throw new ArgumentException("Slot could not be found", "slotId");
//            return new SlotReference(guid, slotId);
//        }

        public T FindSlot<T>(int slotId) where T : GenericSlot
        {
            foreach (var slot in m_Slots)
            {
                if (slot.id == slotId && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        public T FindInputSlot<T>(int slotId) where T : GenericSlot
        {
            foreach (var slot in m_Slots)
            {
                if (slot.isInputSlot && slot.id == slotId && slot is T)
                    return (T) slot;
            }

            return default(T);
        }

        public T FindOutputSlot<T>(int slotId) where T : GenericSlot
        {
            foreach (var slot in m_Slots)
            {
                if (slot.isOutputSlot && slot.id == slotId && slot is T)
                    return (T) slot;
            }

            return default(T);
        }
    }
}
