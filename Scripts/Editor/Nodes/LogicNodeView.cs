using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GeoTetra.GTLogicGraph.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    /// <summary>
    /// Actual visual nodes which gets added to the graph UI.
    /// </summary>
    public class LogicNodeView : Node
    {
        VisualElement _controlsDivider;
        VisualElement _controlItems;
        VisualElement _portInputContainer;
        IEdgeConnectorListener _connectorListener;

        public AbstractLogicNodeEditor LogicNodeEditor { get; private set; }

        public void Initialize(AbstractLogicNodeEditor logicNodeEditor, IEdgeConnectorListener connectorListener)
        {
            this.LoadAndAddStyleSheet("Styles/LogicNodeView");

            if (logicNodeEditor is IInputNode)
                AddToClassList("input");
            else if (logicNodeEditor is IOutputNode)
                AddToClassList("output");

                _connectorListener = connectorListener;
            LogicNodeEditor = logicNodeEditor;
            title = LogicNodeEditor.NodeType();

            var contents = this.Q("contents");

            var controlsContainer = new VisualElement {name = "controls"};
            {
                _controlsDivider = new VisualElement {name = "divider"};
                _controlsDivider.AddToClassList("horizontal");
                controlsContainer.Add(_controlsDivider);
                _controlItems = new VisualElement {name = "items"};
                controlsContainer.Add(_controlItems);

                foreach (var propertyInfo in
                    logicNodeEditor.GetType().GetProperties(BindingFlags.Instance |
                                                            BindingFlags.Public |
                                                            BindingFlags.NonPublic))
                {
                    foreach (INodeControlAttribute attribute in
                        propertyInfo.GetCustomAttributes(typeof(INodeControlAttribute), false))
                    {
                        _controlItems.Add(attribute.InstantiateControl(logicNodeEditor, propertyInfo));
                    }
                }
            }
            contents.Add(controlsContainer);

            // Add port input container, which acts as a pixel cache for all port inputs
            _portInputContainer = new VisualElement
            {
                name = "portInputContainer",
//                clippingOptions = ClippingOptions.ClipAndCacheContents,
                pickingMode = PickingMode.Ignore
            };
            Add(_portInputContainer);

            List<LogicSlot> foundSlots = new List<LogicSlot>();
            logicNodeEditor.GetSlots(foundSlots);
            AddPorts(foundSlots);

            SetPosition(new Rect(logicNodeEditor.Position.x, logicNodeEditor.Position.y, 0, 0));
            UpdatePortInputs();
            base.expanded = logicNodeEditor.Expanded;
            RefreshExpandedState();
            UpdatePortInputVisibilities();
        }

        private void AddPorts(IEnumerable<LogicSlot> slots)
        {
            foreach (var slot in slots)
            {
                var port = LogicPort.Create(slot, _connectorListener);
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

                LogicNodeEditor.Expanded = value;
                RefreshExpandedState(); //This should not be needed. GraphView needs to improve the extension api here
                UpdatePortInputVisibilities();
            }
        }

        void UpdatePortInputs()
        {
            foreach (var port in inputContainer.Children().OfType<LogicPort>())
            {
                if (!_portInputContainer.Children().OfType<PortInputView>().Any(a => Equals(a.Description, port.Slot)))
                {
                    var portInputView = new PortInputView(port.Slot) { style = { position = Position.Absolute } };
                    _portInputContainer.Add(portInputView);
                    if (float.IsNaN(port.layout.width))
                    {
                        port.RegisterCallback<GeometryChangedEvent>(UpdatePortInput);
                    }
                    else
                    {
                        SetPortInputPosition(port, portInputView);
                    }
                }
            }
        }
        
        void SetPortInputPosition(LogicPort port, PortInputView inputView)
        {
            inputView.style.top = port.layout.y;
            inputView.parent.style.height = inputContainer.layout.height;
        }

        
        void UpdatePortInput(GeometryChangedEvent evt)
        {
            var port = (LogicPort)evt.target;
            var inputView = _portInputContainer.Children().OfType<PortInputView>().First(x => Equals(x.Description, port.Slot));
            
            SetPortInputPosition(port, inputView);
            port.UnregisterCallback<GeometryChangedEvent>(UpdatePortInput);
        }

        public void UpdatePortInputVisibilities()
        {
            foreach (var portInputView in _portInputContainer.Children().OfType<PortInputView>())
            {
                var slot = portInputView.Description;
                var oldVisibility = portInputView.visible;
                _foundEdges.Clear();
                LogicNodeEditor.Owner.LogicGraphEditorObject.LogicGraphData.GetEdges(
                    LogicNodeEditor.NodeGuid,
                    portInputView.Description.MemberName,
                    _foundEdges);
                portInputView.visible = expanded && _foundEdges.Count == 0;
                if (portInputView.visible != oldVisibility)
                    _portInputContainer.MarkDirtyRepaint();
            }
        }

        private readonly List<SerializedEdge> _foundEdges = new List<SerializedEdge>();

//        public void UpdatePortInputTypes()
//        {
//            foreach (var anchor in inputContainer.Concat(outputContainer).OfType<LogicPort>())
//            {
//                var slot = anchor.Description;
//                anchor.portName = slot._displayName;
//                anchor.visualClass = slot.ValueType.ToString();
//            }
//
//            foreach (var portInputView in _portInputContainer.OfType<PortInputView>())
//                portInputView.UpdateSlotType();
//
//            foreach (var control in _controlItems)
//            {
//                var listener = control as INodeModificationListener;
//                if (listener != null)
//                    listener.OnNodeModified(ModificationScope.Graph);
//            }
//        }
    }
}