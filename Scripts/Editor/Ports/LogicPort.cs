using System;
using GeoTetra.GTLogicGraph.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    public sealed class LogicPort : Port
    {
        LogicPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            this.LoadAndAddStyleSheet("Styles/LogicPort");
        }

        private LogicSlot _slot;

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
            port.Slot = logicSlot;
            return port;
        }

        public LogicSlot Slot
        {
            get => _slot;
            set
            {
                if (ReferenceEquals(value, _slot))
                    return;
                if (value == null)
                    throw new NullReferenceException();
                if (_slot != null && value.isInputSlot != _slot.isInputSlot)
                    throw new ArgumentException("Cannot change direction of already created port");
                _slot = value;
                portName = Slot.DisplayName;
                visualClass = Slot.ValueType.ToString();
            }
        }
    }
}
