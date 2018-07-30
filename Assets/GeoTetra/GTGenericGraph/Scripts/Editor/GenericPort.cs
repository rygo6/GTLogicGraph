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

        GenericSlot m_Slot;

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
            port.portName = slot.displayName;
            port.visualClass = slot.concreteValueType.ToClassName();
            return port;
        }

        public GenericSlot slot
        {
            get { return m_Slot; }
            set
            {
                if (ReferenceEquals(value, m_Slot))
                    return;
                if (value == null)
                    throw new NullReferenceException();
                if (m_Slot != null && value.isInputSlot != m_Slot.isInputSlot)
                    throw new ArgumentException("Cannot change direction of already created port");
                m_Slot = value;
                portName = slot.displayName;
                visualClass = slot.concreteValueType.ToClassName();
            }
        }
    }

    static class GenericPortExtensions
    {
        public static GenericSlot GetSlot(this Port port)
        {
            var shaderPort = port as GenericPort;
            return shaderPort != null ? shaderPort.slot : null;
        }
    }
}
