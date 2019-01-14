using System;

namespace GeoTetra.GTLogicGraph
{
    public abstract class PortDescription
    {
        private readonly string _memberName;
        private readonly string _displayName = "";
        private readonly PortDirection _portDirection;

        public NodeEditor Owner { get; private set; }
        
        public PortDescription(NodeEditor owner, string memberName, string displayName, PortDirection portDirection)
        {
            Owner = owner;
            _memberName = memberName;
            _displayName = displayName;
            _portDirection = portDirection;
        }

        public string DisplayName
        {
            get { return _displayName + " " + ValueType; }
        }

        public string MemberName
        {
            get { return _memberName; }
        }

        public bool isInputSlot
        {
            get { return _portDirection == PortDirection.Input; }
        }

        public bool isOutputSlot
        {
            get { return _portDirection == PortDirection.Output; }
        }

        public PortDirection PortDirection
        {
            get { return _portDirection; }
        }

        public abstract PortValueType ValueType { get; }

        public abstract bool IsCompatibleWithInputSlotType(PortValueType inputType);

        public bool IsCompatibleWith(PortDescription otherPortDescription)
        {
            return otherPortDescription != null
                   && otherPortDescription.Owner != Owner
                   && otherPortDescription.isInputSlot != isInputSlot
                   && ((isInputSlot
                       ? otherPortDescription.IsCompatibleWithInputSlotType(ValueType)
                       : IsCompatibleWithInputSlotType(otherPortDescription.ValueType)));
        }
    }

    public enum PortDirection
    {
        Input,
        Output
    }
    
    [Serializable]
    public enum PortValueType
    {
        Vector4,
        Vector3,
        Vector2,
        Vector1,
        Boolean,
        CurvePrimitive,
        Mesh,
        VertexList,
    }
}