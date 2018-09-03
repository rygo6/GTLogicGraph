using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEditor.ShaderGraph.Drawing.Controls;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
    [Serializable]
    public struct GenericToggleData
    {
        public bool isOn;

        public GenericToggleData(bool on)
        {
            isOn = on;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class NodeToggleControlAttribute : Attribute, INodeControlAttribute
    {
        string _label;

        public NodeToggleControlAttribute(string label = null)
        {
            _label = label;
        }

        public VisualElement InstantiateControl(NodeDescription nodeDescription, PropertyInfo propertyInfo)
        {
            return new GenericToggleControlView(_label, nodeDescription, propertyInfo);
        }
    }

    public class GenericToggleControlView : VisualElement
    {
        private NodeDescription _nodeDescription;
        private PropertyInfo _propertyInfo;
        private UnityEngine.Experimental.UIElements.Toggle _toggle;

        public GenericToggleControlView(string label, NodeDescription nodeDescription, PropertyInfo propertyInfo)
        {
            _nodeDescription = nodeDescription;
            _propertyInfo = propertyInfo;
            AddStyleSheetPath("Styles/Controls/ToggleControlView");

            if (propertyInfo.PropertyType != typeof(GenericToggleData))
                throw new ArgumentException("Property must be a Toggle.", "propertyInfo");

            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);

            var value = (GenericToggleData)_propertyInfo.GetValue(_nodeDescription, null);
            var panel = new VisualElement { name = "togglePanel" };
            if (!string.IsNullOrEmpty(label))
                panel.Add(new Label(label));
            Action changedToggle = () => { OnChangeToggle(); };
            _toggle = new UnityEngine.Experimental.UIElements.Toggle(changedToggle);
  
            _toggle.on = value.isOn;
            panel.Add(_toggle);
            Add(panel);
        }

        void OnChangeToggle()
        {
            _nodeDescription.Owner.GraphObject.RegisterCompleteObjectUndo("Toggle Change");
            var value = (GenericToggleData)_propertyInfo.GetValue(_nodeDescription, null);
            value.isOn = !value.isOn;
            _propertyInfo.SetValue(_nodeDescription, value, null);
            Dirty(ChangeType.Repaint);
        }
    }
}
