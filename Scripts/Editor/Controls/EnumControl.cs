using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTLogicGraph
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EnumControlAttribute : Attribute, INodeControlAttribute
    {
        private readonly string _label;

        public EnumControlAttribute(string label = null)
        {
            _label = label;
        }

        public VisualElement InstantiateControl(NodeEditor node, PropertyInfo propertyInfo)
        {
            return new EnumControlView(_label, node, propertyInfo);
        }
    }

    public class EnumControlView : VisualElement
    {
        private readonly NodeEditor _nodeEditor;
        private readonly PropertyInfo _propertyInfo;

        public EnumControlView(string label, NodeEditor nodeEditor, PropertyInfo propertyInfo)
        {
            AddStyleSheetPath("Styles/Controls/EnumControlView");
            _nodeEditor = nodeEditor;
            _propertyInfo = propertyInfo;
            if (!propertyInfo.PropertyType.IsEnum)
                throw new ArgumentException("Property must be an enum.", nameof(propertyInfo));
            Add(new Label(label ?? ObjectNames.NicifyVariableName(propertyInfo.Name)));
            var enumField = new EnumField((Enum)_propertyInfo.GetValue(_nodeEditor, null));
            enumField.OnValueChanged(OnValueChanged);
            Add(enumField);
        }

        void OnValueChanged(ChangeEvent<Enum> evt)
        {
            var value = (Enum)_propertyInfo.GetValue(_nodeEditor, null);
            if (!evt.newValue.Equals(value))
            {
                _nodeEditor.Owner.LogicGraphEditorObject.RegisterCompleteObjectUndo("Change " + _nodeEditor.NodeType());
                _propertyInfo.SetValue(_nodeEditor, evt.newValue, null);
            }
        }
    }
}
