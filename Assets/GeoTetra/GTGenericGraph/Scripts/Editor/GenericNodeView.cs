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
    /// <summary>
    /// Actual visual nodes which gets added to the graph UI.
    /// </summary>
    public class GenericNodeView : Node
    {
        VisualElement _controlsDivider;
        VisualElement _controlItems;
        VisualElement _portInputContainer;
        IEdgeConnectorListener _connectorListener;

        public NodeEditor NodeEditor { get; private set; }

        public void Initialize(NodeEditor nodeEditor, IEdgeConnectorListener connectorListener)
        {
            AddStyleSheetPath("Styles/GenericNodeView");

            _connectorListener = connectorListener;
            NodeEditor = nodeEditor;
            title = NodeEditor.NodeType();

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
                foreach (var propertyInfo in nodeEditor.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                foreach (IGenericControlAttribute attribute in propertyInfo.GetCustomAttributes(
                    typeof(IGenericControlAttribute), false))
                    _controlItems.Add(attribute.InstantiateControl(nodeEditor, propertyInfo));
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

            List<GenericSlot> foundSlots = new List<GenericSlot>();
            nodeEditor.GetSlots(foundSlots);
            AddSlots(foundSlots);
            UpdatePortInputs();
            expanded = nodeEditor.Expanded;
            RefreshExpandedState(); //This should not be needed. GraphView needs to improve the extension api here
            UpdatePortInputVisibilities();

            _portInputContainer.SendToBack();

            SetPosition(new Rect(nodeEditor.Position.x, nodeEditor.Position.y, 0, 0));

            RefreshExpandedState();
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
                if (!_portInputContainer.OfType<GenericPortInputView>().Any(a => Equals(a.Slot, port.slot)))
                {
                    var portInputView = new GenericPortInputView(port.slot)
                    {
                        style = {positionType = PositionType.Absolute}
                    };
                    _portInputContainer.Add(portInputView);
                    port.RegisterCallback<GeometryChangedEvent>(evt => UpdatePortInput((GenericPort) evt.target));
                }
            }
        }

        void UpdatePortInput(GenericPort port)
        {
            var inputView = _portInputContainer.OfType<GenericPortInputView>().First(x => Equals(x.Slot, port.slot));

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
            _portInputContainer.Dirty(ChangeType.Repaint);
//            foreach (var portInputView in m_PortInputContainer.OfType<GenericPortInputView>())
//            {
//                var slot = portInputView.Slot;
//                var oldVisibility = portInputView.visible;
//                portInputView.visible = expanded && !NodeEditor.Owner.GetEdges(NodeEditor.GetSlotReference(slot.id)).Any();
//                if (portInputView.visible != oldVisibility)
//                    m_PortInputContainer.Dirty(ChangeType.Repaint);
//            }
        }
    }
}