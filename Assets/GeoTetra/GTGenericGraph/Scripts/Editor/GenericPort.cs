using System;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
    sealed class GenericPort : Port
    {
#if UNITY_2018_1
        GenericPort(Orientation portOrientation, Direction portDirection, Type type)
            : base(portOrientation, portDirection, type)
#else
        ShaderPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
#endif
        {
            AddStyleSheetPath("Styles/GenericPort");
        }

        GenericSlot _slot;

        public static Port Create(GenericSlot slot, IEdgeConnectorListener connectorListener)
        {
            var port = new GenericPort(Orientation.Horizontal, slot.isInputSlot ? Direction.Input : Direction.Output,
#if !UNITY_2018_1
                slot.isInputSlot ? Capacity.Single : Capacity.Multi,
#endif
                null)
            {
                m_EdgeConnector = new EdgeConnector<Edge>(connectorListener),
            };
            port.AddManipulator(port.m_EdgeConnector);
            port.Slot = slot;
            port.portName = slot.displayName;
            port.visualClass = slot.concreteValueType.ToClassName();
            return port;
        }

        public GenericSlot Slot
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
                portName = Slot.displayName;
                visualClass = Slot.concreteValueType.ToClassName();
            }
        }
    }

    static class GenericPortExtensions
    {
        public static GenericSlot GetSlot(this Port port)
        {
            var shaderPort = port as GenericPort;
            return shaderPort != null ? shaderPort.Slot : null;
        }
    }
}
