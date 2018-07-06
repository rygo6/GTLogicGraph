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
        VisualElement _controlsDivider;
        VisualElement _controlItems;
        VisualElement _portInputContainer;
        IEdgeConnectorListener _connectorListener;

        public AbstractGenericNode Node { get; private set; }

        public void Initialize(AbstractGenericNode node)
        {
            AddStyleSheetPath("Styles/GenericNodeView");

            this.Node = node;

            title = "Node";

            var contents = this.Q("contents");

            var controlsContainer = new VisualElement {name = "controls"};
            {
                _controlsDivider = new VisualElement {name = "divider"};
                _controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(_controlsDivider);
                _controlItems = new VisualElement {name = "items"};
                controlsContainer.Add(_controlItems);

                //TODO replicate generic control attribute classes
                foreach (var propertyInfo in node.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                foreach (IGenericControlAttribute attribute in propertyInfo.GetCustomAttributes(
                    typeof(IGenericControlAttribute), false))
                    _controlItems.Add(attribute.InstantiateControl(node, propertyInfo));

                for (int i = 0; i < 8; ++i)
                {
                    _controlItems.Add(new Label("controls " + i));
//                    outputContainer.Add(new Label("out " + i));
//                    inputContainer.Add(new Label("in " + i));
                }
            }

            contents.Add(controlsContainer);

            // Add port input container, which acts as a pixel cache for all port inputs
            _portInputContainer = new VisualElement
            {
                name = "portInputContainer",
                clippingOptions = ClippingOptions.ClipAndCacheContents,
                pickingMode = PickingMode.Ignore
            };
            Add(_portInputContainer);

            AddSlots(node.GetSlots<GenericSlot>());
            UpdatePortInputs();
            base.expanded = node.DrawState.expanded;
            RefreshExpandedState(); //This should not be needed. GraphView needs to improve the extension api here
//            UpdatePortInputVisibilities();
            
            _portInputContainer.SendToBack();

            SetPosition(new Rect(node.DrawState.position.x, node.DrawState.position.y, 0, 0));

            RefreshExpandedState();
        }
        
        void UpdateTitle()
        {
//            var subGraphNode = Node as SubGraphNode;
//            if (subGraphNode != null && subGraphNode.subGraphAsset != null)
//                title = subGraphNode.subGraphAsset.name;
//            else
                title = Node.name;
        }
        
        public void OnModified(ModificationScope scope)
        {
            UpdateTitle();

            base.expanded = Node.DrawState.expanded;

            // Update slots to match node modification
            if (scope == ModificationScope.Topological)
            {
                var slots = Node.GetSlots<GenericSlot>().ToList();

                var inputPorts = inputContainer.Children().OfType<GenericPort>().ToList();
                foreach (var port in inputPorts)
                {
                    var currentSlot = port.Slot;
                    var newSlot = slots.FirstOrDefault(s => s.id == currentSlot.id);
                    if (newSlot == null)
                    {
                        // Slot doesn't exist anymore, remove it
                        inputContainer.Remove(port);

                        // We also need to remove the inline input
                        var portInputView = _portInputContainer.OfType<GenericPortInputView>().FirstOrDefault(v => Equals(v.Slot, port.Slot));
                        if (portInputView != null)
                            portInputView.RemoveFromHierarchy();
                    }
                    else
                    {
                        port.Slot = newSlot;
                        var portInputView = _portInputContainer.OfType<GenericPortInputView>().FirstOrDefault(x => x.Slot.id == currentSlot.id);
                        portInputView.UpdateSlot(newSlot);

                        slots.Remove(newSlot);
                    }
                }

                var outputPorts = outputContainer.Children().OfType<GenericPort>().ToList();
                foreach (var port in outputPorts)
                {
                    var currentSlot = port.Slot;
                    var newSlot = slots.FirstOrDefault(s => s.id == currentSlot.id);
                    if (newSlot == null)
                    {
                        outputContainer.Remove(port);
                    }
                    else
                    {
                        port.Slot = newSlot;
                        slots.Remove(newSlot);
                    }
                }

                AddSlots(slots);

                slots.Clear();
                slots.AddRange(Node.GetSlots<GenericSlot>());

                if (inputContainer.childCount > 0)
                    inputContainer.Sort((x, y) => slots.IndexOf(((GenericPort)x).Slot) - slots.IndexOf(((GenericPort)y).Slot));
                if (outputContainer.childCount > 0)
                    outputContainer.Sort((x, y) => slots.IndexOf(((GenericPort)x).Slot) - slots.IndexOf(((GenericPort)y).Slot));
            }

            RefreshExpandedState(); //This should not be needed. GraphView needs to improve the extension api here
            UpdatePortInputs();
            UpdatePortInputVisibilities();

            foreach (var control in _controlItems)
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

                var port = GenericPort.Create(slot, _connectorListener);
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
                if (!_portInputContainer.OfType<GenericPortInputView>().Any(a => Equals(a.Slot, port.Slot)))
                {
                    var portInputView =
                        new GenericPortInputView(port.Slot) {style = {positionType = PositionType.Absolute}};
                    _portInputContainer.Add(portInputView);
                    port.RegisterCallback<PostLayoutEvent>(evt => UpdatePortInput((GenericPort) evt.target));
                }
            }
        }

        void UpdatePortInput(GenericPort port)
        {
            var inputView = _portInputContainer.OfType<GenericPortInputView>().First(x => Equals(x.Slot, port.Slot));

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
            foreach (var portInputView in _portInputContainer.OfType<GenericPortInputView>())
            {
                var slot = portInputView.Slot;
                var oldVisibility = portInputView.visible;
                portInputView.visible = expanded && !Node.owner.GetEdges(Node.GetSlotReference(slot.id)).Any();
                if (portInputView.visible != oldVisibility)
                    _portInputContainer.Dirty(ChangeType.Repaint);
            }
        }


        public void UpdatePortInputTypes()
        {
            foreach (var anchor in inputContainer.Concat(outputContainer).OfType<GenericPort>())
            {
                var slot = anchor.Slot;
                anchor.portName = slot.displayName;
                anchor.visualClass = slot.concreteValueType.ToClassName();
            }

            foreach (var portInputView in _portInputContainer.OfType<GenericPortInputView>())
                portInputView.UpdateSlotType();

            foreach (var control in _controlItems)
            {
                var listener = control as INodeModificationListener;
                if (listener != null)
                    listener.OnNodeModified(ModificationScope.Graph);
            }
        }
    }
}