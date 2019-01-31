using System;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTLogicGraph
{
    public sealed class LogicPort : Port
    {
        LogicPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            AddStyleSheetPath("Styles/LogicPort");
        }

        private PortDescription _description;

        public static Port Create(PortDescription portDescription, IEdgeConnectorListener connectorListener)
        {
            var port = new LogicPort(Orientation.Horizontal, 
                portDescription.isInputSlot ? Direction.Input : Direction.Output,
                portDescription.isInputSlot ? Capacity.Single : Capacity.Multi,
                null)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.Description = portDescription;
            return port;
        }

        public PortDescription Description
        {
            get => _description;
            set
            {
                if (ReferenceEquals(value, _description))
                    return;
                if (value == null)
                    throw new NullReferenceException();
                if (_description != null && value.isInputSlot != _description.isInputSlot)
                    throw new ArgumentException("Cannot change direction of already created port");
                _description = value;
                portName = Description.DisplayName;
                visualClass = Description.ValueType.ToString();
            }
        }
    }
}
