using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine.Experimental.UIElements;

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

        public NodeDescription NodeDescription { get; private set; }

        public void Initialize(NodeDescription nodeDescription, IEdgeConnectorListener connectorListener)
        {
            AddStyleSheetPath("Styles/GenericNodeView");
            AddToClassList("GenericNode");

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

                foreach (var propertyInfo in
                    nodeDescription.GetType().GetProperties(BindingFlags.Instance |
                                                            BindingFlags.Public |
                                                            BindingFlags.NonPublic))
                {
                    foreach (INodeControlAttribute attribute in
                        propertyInfo.GetCustomAttributes(typeof(INodeControlAttribute), false))
                    {
                        _controlItems.Add(attribute.InstantiateControl(nodeDescription, propertyInfo));
                    }
                }
            }
            contents.Add(controlsContainer);

            List<PortDescription> foundSlots = new List<PortDescription>();
            nodeDescription.GetSlots(foundSlots);
            AddSlots(foundSlots);

            SetPosition(new Rect(nodeDescription.Position.x, nodeDescription.Position.y, 0, 0));
            base.expanded = nodeDescription.Expanded;
            RefreshExpandedState();
        }

        private void AddSlots(IEnumerable<PortDescription> slots)
        {
            foreach (var slot in slots)
            {
                var port = PortView.Create(slot, _connectorListener);
                if (slot.isOutputSlot)
                    outputContainer.Add(port);
                else
                    inputContainer.Add(port);
            }
        }
        
        public override bool expanded
        {
            get { return base.expanded; }
            set
            {
                Debug.Log(value);
                if (base.expanded != value)
                    base.expanded = value;

                NodeDescription.Expanded = value;
                RefreshExpandedState(); //This should not be needed. GraphView needs to improve the extension api here
            }
        }
    }
}