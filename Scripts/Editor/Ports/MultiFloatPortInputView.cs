using System;
using System.Globalization;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEditor.Graphing;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTLogicGraph.Ports
{
    public class MultiFloatPortInputView : VisualElement
    {
        readonly LogicNodeEditor _node;
        readonly Func<Vector4> _get;
        readonly Action<Vector4> _set;
        int _undoGroup = -1;

        public MultiFloatPortInputView(LogicNodeEditor node, string[] labels, Func<Vector4> get, Action<Vector4> set)
        {
            AddStyleSheetPath("Styles/Controls/MultiFloatPortInputView");
            _node = node;
            _get = get;
            _set = set;
            var initialValue = get();
            for (var i = 0; i < labels.Length; i++)
                AddField(initialValue, i, labels[i]);
        }

        void AddField(Vector4 initialValue, int index, string subLabel)
        {
            var dummy = new VisualElement { name = "dummy" };
            var label = new Label(subLabel);
            dummy.Add(label);
            Add(dummy);
            var field = new FloatField { userData = index, value = initialValue[index] };
            var dragger = new FieldMouseDragger<float>(field);
            dragger.SetDragZone(label);
            field.OnValueChanged(evt =>
                {
                    var value = _get();
                    value[index] = (float)evt.newValue;
                    _set(value);
                    _node.SetDirty();
                    _undoGroup = -1;
                });
            field.RegisterCallback<InputEvent>(evt =>
                {
                    if (_undoGroup == -1)
                    {
                        _undoGroup = Undo.GetCurrentGroup();
                        _node.Owner.LogicGraphEditorObject.RegisterCompleteObjectUndo("Change " + _node.NodeType());
                    }
                    if (!float.TryParse(evt.newData, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out var newValue))
                        newValue = 0f;
                    var value = _get();
                    if (Math.Abs(value[index] - newValue) > 1e-9)
                    {
                        value[index] = newValue;
                        _set(value);
                        _node.SetDirty();
                    }
                });
            field.RegisterCallback<KeyDownEvent>(evt =>
                {
                    if (evt.keyCode == KeyCode.Escape && _undoGroup > -1)
                    {
                        Undo.RevertAllDownToGroup(_undoGroup);
                        _undoGroup = -1;
                        evt.StopPropagation();
                    }
                    MarkDirtyRepaint();
                });
            Add(field);
        }
    }
}
