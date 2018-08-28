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
    public class GenericNodeView : UnityEditor.Experimental.UIElements.GraphView.Node
    {
        VisualElement _controlsDivider;
        VisualElement _controlItems;
        VisualElement _portInputContainer;
        IEdgeConnectorListener _connectorListener;

        public NodeDescription NodeDescription { get; private set; }

        public void Initialize(NodeDescription nodeDescription, IEdgeConnectorListener connectorListener)
        {
            AddStyleSheetPath("Styles/GenericNodeView");

            _connectorListener = connectorListener;
            NodeDescription = nodeDescription;
            title = NodeDescription.NodeType();

            var contents = this.Q("contents");

            var controlsContainer = new VisualElement {name = "controls"};
            {
                _controlsDivider = new VisualElement {name = "divider"};
                _controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(_controlsDivider);
                _controlItems = new VisualElement {name = "items"};
                controlsContainer.Add(_controlItems);

                //TODO replicate generic control attribute classes
                foreach (var propertyInfo in nodeDescription.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                foreach (IGenericControlAttribute attribute in propertyInfo.GetCustomAttributes(
                    typeof(IGenericControlAttribute), false))
                    _controlItems.Add(attribute.InstantiateControl(nodeDescription, propertyInfo));
            }
            contents.Add(controlsContainer);

            List<GenericPortDescription> foundSlots = new List<GenericPortDescription>();
            nodeDescription.GetSlots(foundSlots);
            AddSlots(foundSlots);

            SetPosition(new Rect(nodeDescription.Position.x, nodeDescription.Position.y, 0, 0));

            RefreshExpandedState();
        }

        private void AddSlots(IEnumerable<GenericPortDescription> slots)
        {
            foreach (var slot in slots)
            {
                var port = GenericPort.Create(slot, _connectorListener);
                if (slot.isOutputSlot)
                    outputContainer.Add(port);
                else
                    inputContainer.Add(port);
            }
        }
    }
}