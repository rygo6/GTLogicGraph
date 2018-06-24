using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
    public class GenericNodeView : Node
    {
        VisualElement _controlsDivider;
        VisualElement _controlItems;
        IEdgeConnectorListener _connectorListener;

        public AbstractGenericNode Node { get; private set; }
        
        public void Initialize(AbstractGenericNode node)
        {
            AddStyleSheetPath("Styles/GenericNodeView");

            Node = node;
            
            title = "Node";

            var contents = this.Q("contents");

            var controlsContainer = new VisualElement {name = "controls"};
            {
                _controlsDivider = new VisualElement {name = "divider"};
                _controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(_controlsDivider);
                _controlItems = new VisualElement {name = "items"};
                controlsContainer.Add(_controlItems);

                for (int i = 0; i < 8; ++i)
                {
                    _controlItems.Add(new Label("controls " + i));
                    outputContainer.Add(new Label("out " + i));
                    inputContainer.Add(new Label("in " + i));
                }
            }

            contents.Add(controlsContainer);

            SetPosition(new Rect(node.DrawState.position.x, node.DrawState.position.y, 0, 0));

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
    }
}