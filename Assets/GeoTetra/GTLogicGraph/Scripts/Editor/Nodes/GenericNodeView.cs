using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTLogicGraph
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

            var contents = this.Q("contents");

            var controlsContainer = new VisualElement {name = "controls"};
            {
                _controlsDivider = new VisualElement {name = "divider"};
                _controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(_controlsDivider);
                _controlItems = new VisualElement {name = "items"};
                controlsContainer.Add(_controlItems);

                foreach (var propertyInfo in
                    nodeEditor.GetType().GetProperties(BindingFlags.Instance |
                                                            BindingFlags.Public |
                                                            BindingFlags.NonPublic))
                {
                    foreach (INodeControlAttribute attribute in
                        propertyInfo.GetCustomAttributes(typeof(INodeControlAttribute), false))
                    {
                        _controlItems.Add(attribute.InstantiateControl(nodeEditor, propertyInfo));
                    }
                }
            }
            contents.Add(controlsContainer);

            List<PortDescription> foundSlots = new List<PortDescription>();
            nodeEditor.GetSlots(foundSlots);
            AddSlots(foundSlots);

            SetPosition(new Rect(nodeEditor.Position.x, nodeEditor.Position.y, 0, 0));
            base.expanded = nodeEditor.Expanded;
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

                NodeEditor.Expanded = value;
                RefreshExpandedState(); //This should not be needed. GraphView needs to improve the extension api here
            }
        }
    }
}