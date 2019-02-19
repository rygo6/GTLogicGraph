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

        private LogicSlot _description;

        public static Port Create(LogicSlot logicSlot, IEdgeConnectorListener connectorListener)
        {
            var port = new LogicPort(Orientation.Horizontal, 
                logicSlot.isInputSlot ? Direction.Input : Direction.Output,
                logicSlot.isInputSlot ? Capacity.Single : Capacity.Multi,
                null)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.Description = logicSlot;
            return port;
        }

        public LogicSlot Description
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
