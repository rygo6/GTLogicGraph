using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleEnums;

namespace GeoTetra.GTGenericGraph
{
    public class GenericNodeView : Node
    {
        VisualElement m_ControlsDivider;
        VisualElement m_ControlItems;
        VisualElement m_PortInputContainer;
        IEdgeConnectorListener m_ConnectorListener;

        public AbstractGenericNode node { get; private set; }

        public void Initialize(AbstractGenericNode node, IEdgeConnectorListener connectorListener)
        {
            AddStyleSheetPath("Styles/GenericNodeView");

            m_ConnectorListener = connectorListener;
            this.node = node;

            title = "Node";

            var contents = this.Q("contents");

            var controlsContainer = new VisualElement {name = "controls"};
            {
                m_ControlsDivider = new VisualElement {name = "divider"};
                m_ControlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(m_ControlsDivider);
                m_ControlItems = new VisualElement {name = "items"};
                controlsContainer.Add(m_ControlItems);

                //TODO replicate generic control attribute classes
                foreach (var propertyInfo in node.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                foreach (IGenericControlAttribute attribute in propertyInfo.GetCustomAttributes(
                    typeof(IGenericControlAttribute), false))
                    m_ControlItems.Add(attribute.InstantiateControl(node, propertyInfo));
            }

            contents.Add(controlsContainer);

            // Add port input container, which acts as a pixel cache for all port inputs
            m_PortInputContainer = new VisualElement
            {
                name = "portInputContainer",
                clippingOptions = ClippingOptions.ClipAndCacheContents,
                pickingMode = PickingMode.Ignore
            };
            Add(m_PortInputContainer);

            AddSlots(node.GetSlots<GenericSlot>());
            UpdatePortInputs();
            base.expanded = node.drawState.expanded;
            RefreshExpandedState(); //This should not be needed. GraphView needs to improve the extension api here
            UpdatePortInputVisibilities();
            
            m_PortInputContainer.SendToBack();

            SetPosition(new Rect(node.drawState.position.x, node.drawState.position.y, 0, 0));

            RefreshExpandedState();
        }
        
        void UpdateTitle()
        {
//            var subGraphNode = Node as SubGraphNode;
//            if (subGraphNode != null && subGraphNode.subGraphAsset != null)
//                title = subGraphNode.subGraphAsset.name;
//            else
                title = node.name;
        }
        
        public void OnModified(ModificationScope scope)
        {
            UpdateTitle();

            base.expanded = node.drawState.expanded;

            // Update slots to match node modification
            if (scope == ModificationScope.Topological)
            {
                var slots = node.GetSlots<GenericSlot>().ToList();

                var inputPorts = inputContainer.Children().OfType<GenericPort>().ToList();
                foreach (var port in inputPorts)
                {
                    var currentSlot = port.slot;
                    var newSlot = slots.FirstOrDefault(s => s.id == currentSlot.id);
                    if (newSlot == null)
                    {
                        // Slot doesn't exist anymore, remove it
                        inputContainer.Remove(port);

                        // We also need to remove the inline input
                        var portInputView = m_PortInputContainer.OfType<GenericPortInputView>().FirstOrDefault(v => Equals(v.Slot, port.slot));
                        if (portInputView != null)
                            portInputView.RemoveFromHierarchy();
                    }
                    else
                    {
                        port.slot = newSlot;
                        var portInputView = m_PortInputContainer.OfType<GenericPortInputView>().FirstOrDefault(x => x.Slot.id == currentSlot.id);
                        portInputView.UpdateSlot(newSlot);

                        slots.Remove(newSlot);
                    }
                }

                var outputPorts = outputContainer.Children().OfType<GenericPort>().ToList();
                foreach (var port in outputPorts)
                {
                    var currentSlot = port.slot;
                    var newSlot = slots.FirstOrDefault(s => s.id == currentSlot.id);
                    if (newSlot == null)
                    {
                        outputContainer.Remove(port);
                    }
                    else
                    {
                        port.slot = newSlot;
                        slots.Remove(newSlot);
                    }
                }

                AddSlots(slots);

                slots.Clear();
                slots.AddRange(node.GetSlots<GenericSlot>());

                if (inputContainer.childCount > 0)
                    inputContainer.Sort((x, y) => slots.IndexOf(((GenericPort)x).slot) - slots.IndexOf(((GenericPort)y).slot));
                if (outputContainer.childCount > 0)
                    outputContainer.Sort((x, y) => slots.IndexOf(((GenericPort)x).slot) - slots.IndexOf(((GenericPort)y).slot));
            }

            RefreshExpandedState(); //This should not be needed. GraphView needs to improve the extension api here
            UpdatePortInputs();
            UpdatePortInputVisibilities();

            foreach (var control in m_ControlItems)
            {
                var listener = control as INodeModificationListener;
                if (listener != null)
                    listener.OnNodeModified(scope);
            }
        }

        void AddSlots(IEnumerable<GenericSlot> slots)
        {
            foreach (var slot in slots)
            {
                if (slot.hidden)
                    continue;

                var port = GenericPort.Create(slot, m_ConnectorListener);
                if (slot.isOutputSlot)
                    outputContainer.Add(port);
                else
                    inputContainer.Add(port);
            }
        }

        void UpdatePortInputs()
        {
            foreach (var port in inputContainer.OfType<GenericPort>())
            {
                if (!m_PortInputContainer.OfType<GenericPortInputView>().Any(a => Equals(a.Slot, port.slot)))
                {
                    var portInputView =
                        new GenericPortInputView(port.slot) {style = {positionType = PositionType.Absolute}};
                    m_PortInputContainer.Add(portInputView);
                    port.RegisterCallback<GeometryChangedEvent>(evt => UpdatePortInput((GenericPort) evt.target));
                }
            }
        }

        void UpdatePortInput(GenericPort port)
        {
            var inputView = m_PortInputContainer.OfType<GenericPortInputView>().First(x => Equals(x.Slot, port.slot));

            var currentRect = new Rect(inputView.style.positionLeft, inputView.style.positionTop, inputView.style.width,
                inputView.style.height);
            var targetRect = new Rect(0.0f, 0.0f, port.layout.width, port.layout.height);
            targetRect = port.ChangeCoordinatesTo(inputView.shadow.parent, targetRect);
            var centerY = targetRect.center.y;
            var centerX = targetRect.xMax - currentRect.width;
            currentRect.center = new Vector2(centerX, centerY);

            inputView.style.positionTop = currentRect.yMin;
            var newHeight = inputView.parent.layout.height;
            foreach (var element in inputView.parent.Children())
                newHeight = Mathf.Max(newHeight, element.style.positionTop + element.layout.height);
            if (Math.Abs(inputView.parent.style.height - newHeight) > 1e-3)
                inputView.parent.style.height = newHeight;
        }

        public void UpdatePortInputVisibilities()
        {
            foreach (var portInputView in m_PortInputContainer.OfType<GenericPortInputView>())
            {
                var slot = portInputView.Slot;
                var oldVisibility = portInputView.visible;
                portInputView.visible = expanded && !node.owner.GetEdges(node.GetSlotReference(slot.id)).Any();
                if (portInputView.visible != oldVisibility)
                    m_PortInputContainer.Dirty(ChangeType.Repaint);
            }
        }


        public void UpdatePortInputTypes()
        {
            foreach (var anchor in inputContainer.Concat(outputContainer).OfType<GenericPort>())
            {
                var slot = anchor.slot;
                anchor.portName = slot.displayName;
                anchor.visualClass = slot.concreteValueType.ToClassName();
            }

            foreach (var portInputView in m_PortInputContainer.OfType<GenericPortInputView>())
                portInputView.UpdateSlotType();

            foreach (var control in m_ControlItems)
            {
                var listener = control as INodeModificationListener;
                if (listener != null)
                    listener.OnNodeModified(ModificationScope.Graph);
            }
        }
    }
}