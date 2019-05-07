using System;
using System.Reflection;
using GeoTetra.GTLogicGraph.Extensions;
using UnityEditor;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NodeToggleControlAttribute : Attribute, INodeControlAttribute
    {
        private readonly string _label;

        public NodeToggleControlAttribute(string label = null)
        {
            _label = label;
        }

        public VisualElement InstantiateControl(AbstractLogicNodeEditor logicNodeEditor, PropertyInfo propertyInfo)
        {
            return new ToggleControlView(_label, logicNodeEditor, propertyInfo);
        }
    }

    public class ToggleControlView : VisualElement
    {
        private AbstractLogicNodeEditor _logicNodeEditor;
        private PropertyInfo _propertyInfo;
        private Toggle _toggle;

        public ToggleControlView(string label, AbstractLogicNodeEditor logicNodeEditor, PropertyInfo propertyInfo)
        {
            _logicNodeEditor = logicNodeEditor;
            _propertyInfo = propertyInfo;
            this.LoadAndAddStyleSheet("Styles/Controls/ToggleControlView");

            if (propertyInfo.PropertyType != typeof(bool))
                throw new ArgumentException("Property must be a Toggle.", "propertyInfo");

            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);

            var value = (bool)_propertyInfo.GetValue(_logicNodeEditor, null);
            var panel = new VisualElement { name = "togglePanel" };
            if (!string.IsNullOrEmpty(label))
                panel.Add(new Label(label));
            _toggle = new Toggle {value = value};
            _toggle.RegisterValueChangedCallback(OnChangeToggle);

            panel.Add(_toggle);
            Add(panel);
        }

        private void OnChangeToggle(ChangeEvent<bool> evt)
        {
            _logicNodeEditor.Owner.LogicGraphEditorObject.RegisterCompleteObjectUndo("Toggle Change " + _logicNodeEditor.NodeType());
            var value = (bool)_propertyInfo.GetValue(_logicNodeEditor, null);
            value = !value;
            _propertyInfo.SetValue(_logicNodeEditor, value, null);
            MarkDirtyRepaint();            
        }
    }
}
