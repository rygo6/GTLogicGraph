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
        public bool isEnabled;

        public GenericToggleData(bool on, bool enabled)
        {
            isOn = on;
            isEnabled = enabled;
        }

        public GenericToggleData(bool on)
        {
            isOn = on;
            isEnabled = true;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class GenericToggleControlAttribute : Attribute, IGenericControlAttribute
    {
        string m_Label;

        public GenericToggleControlAttribute(string label = null)
        {
            m_Label = label;
        }

        public VisualElement InstantiateControl(NodeEditor nodeEditor, PropertyInfo propertyInfo)
        {
            return new GenericToggleControlView(m_Label, nodeEditor, propertyInfo);
        }
    }

    public class GenericToggleControlView : VisualElement, INodeModificationListener
    {
        NodeEditor _nodeEditor;
        PropertyInfo m_PropertyInfo;

        UnityEngine.Experimental.UIElements.Toggle m_Toggle;

        public GenericToggleControlView(string label, NodeEditor nodeEditor, PropertyInfo propertyInfo)
        {
            _nodeEditor = nodeEditor;
            m_PropertyInfo = propertyInfo;
            AddStyleSheetPath("Styles/Controls/ToggleControlView");

            if (propertyInfo.PropertyType != typeof(GenericToggleData))
                throw new ArgumentException("Property must be a Toggle.", "propertyInfo");

            label = label ?? ObjectNames.NicifyVariableName(propertyInfo.Name);

            var value = (GenericToggleData)m_PropertyInfo.GetValue(_nodeEditor, null);
            var panel = new VisualElement { name = "togglePanel" };
            if (!string.IsNullOrEmpty(label))
                panel.Add(new Label(label));
            Action changedToggle = () => { OnChangeToggle(); };
            m_Toggle = new UnityEngine.Experimental.UIElements.Toggle(changedToggle);
            m_Toggle.SetEnabled(value.isEnabled);
            m_Toggle.on = value.isOn;
            panel.Add(m_Toggle);
            Add(panel);
        }

        public void OnNodeModified(ModificationScope scope)
        {
            var value = (GenericToggleData)m_PropertyInfo.GetValue(_nodeEditor, null);
            m_Toggle.SetEnabled(value.isEnabled);

            if (scope == ModificationScope.Graph)
            {
                this.Dirty(ChangeType.Repaint);
            }
        }

        void OnChangeToggle()
        {
            _nodeEditor.Owner.Graph.RegisterCompleteObjectUndo("Toggle Change");
            var value = (GenericToggleData)m_PropertyInfo.GetValue(_nodeEditor, null);
            value.isOn = !value.isOn;
            m_PropertyInfo.SetValue(_nodeEditor, value, null);
            this.Dirty(ChangeType.Repaint);
        }
    }
}
