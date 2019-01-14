using System;
using System.Reflection;
using UnityEditor;
using UnityEngine.Experimental.UIElements;

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

        public VisualElement InstantiateControl(NodeEditor nodeEditor, PropertyInfo propertyInfo)
        {
            return new ToggleControlView(_label, nodeEditor, propertyInfo);
        }
    }

    public class ToggleControlView : VisualElement
    {
        private NodeEditor _nodeEditor;
        private PropertyInfo _propertyInfo;
        private Toggle _toggle;

        public ToggleControlView(string label, NodeEditor nodeEditor, PropertyInfo propertyInfo)
        {
            _nodeEditor = nodeEditor;
            _propertyInfo = propertyInfo;
            AddStyleSheetPath("Styles/Controls/ToggleControlView");

            if (propertyInfo.PropertyType != typeof(bool))
                throw new ArgumentException("Property must be a Toggle.", "propertyInfo");

            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);

            var value = (bool)_propertyInfo.GetValue(_nodeEditor, null);
            var panel = new VisualElement { name = "togglePanel" };
            if (!string.IsNullOrEmpty(label))
                panel.Add(new Label(label));
            Action changedToggle = () => { OnChangeToggle(); };
            _toggle = new Toggle(changedToggle);
  
            _toggle.value = value;
            panel.Add(_toggle);
            Add(panel);
        }

        private void OnChangeToggle()
        {
            _nodeEditor.Owner.LogicGraphEditorObject.RegisterCompleteObjectUndo("Toggle Change " + _nodeEditor.NodeType());
            var value = (bool)_propertyInfo.GetValue(_nodeEditor, null);
            value = !value;
            _propertyInfo.SetValue(_nodeEditor, value, null);
            Dirty(ChangeType.Repaint);
        }
    }
}
