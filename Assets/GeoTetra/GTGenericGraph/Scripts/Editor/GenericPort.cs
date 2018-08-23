using System;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
    public sealed class GenericPort : Port
    {
        GenericPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            AddStyleSheetPath("Styles/GenericPort");
        }

        GenericSlot _slot;

        public static Port Create(GenericSlot slot, IEdgeConnectorListener connectorListener)
        {
            var port = new GenericPort(Orientation.Horizontal, 
                slot.isInputSlot ? Direction.Input : Direction.Output,
                slot.isInputSlot ? Capacity.Single : Capacity.Multi,
                null)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.slot = slot;
            port.portName = slot.DisplayName;
            port.visualClass = slot.concreteValueType.ToClassName();
            return port;
        }

        public GenericSlot slot
        {
            get { return _slot; }
            set
            {
                if (ReferenceEquals(value, _slot))
                    return;
                if (value == null)
                    throw new NullReferenceException();
                if (_slot != null && value.isInputSlot != _slot.isInputSlot)
                    throw new ArgumentException("Cannot change direction of already created port");
                _slot = value;
                portName = slot.DisplayName;
                visualClass = slot.concreteValueType.ToClassName();
            }
        }
    }
}
