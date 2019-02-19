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

        public VisualElement InstantiateControl(AbstractLogicNodeEditor logicNodeEditor, PropertyInfo propertyInfo)
        {
            return new EnumControlView(_label, logicNodeEditor, propertyInfo);
        }
    }

    public class EnumControlView : VisualElement
    {
        private readonly AbstractLogicNodeEditor _logicNodeEditor;
        private readonly PropertyInfo _propertyInfo;

        public EnumControlView(string label, AbstractLogicNodeEditor logicNodeEditor, PropertyInfo propertyInfo)
        {
            AddStyleSheetPath("Styles/Controls/EnumControlView");
            _logicNodeEditor = logicNodeEditor;
            _propertyInfo = propertyInfo;
            if (!propertyInfo.PropertyType.IsEnum)
                throw new ArgumentException("Property must be an enum.", nameof(propertyInfo));
            Add(new Label(label ?? ObjectNames.NicifyVariableName(propertyInfo.Name)));
            var enumField = new EnumField((Enum)_propertyInfo.GetValue(_logicNodeEditor, null));
            enumField.OnValueChanged(OnValueChanged);
            Add(enumField);
        }

        void OnValueChanged(ChangeEvent<Enum> evt)
        {
            var value = (Enum)_propertyInfo.GetValue(_logicNodeEditor, null);
            if (!evt.newValue.Equals(value))
            {
                _logicNodeEditor.Owner.LogicGraphEditorObject.RegisterCompleteObjectUndo("Change " + _logicNodeEditor.NodeType());
                _propertyInfo.SetValue(_logicNodeEditor, evt.newValue, null);
            }
        }
    }
}
