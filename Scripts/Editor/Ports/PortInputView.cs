using System;
using GeoTetra.GTLogicGraph.Extensions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
    public class PortInputView : GraphElement, IDisposable
    {
        readonly CustomStyleProperty<Color> k_EdgeColorProperty = new CustomStyleProperty<Color>("--edge-color");

        Color _edgeColor = Color.red;
       
        public Color edgeColor
        {
            get { return _edgeColor; }
        }

        public LogicSlot Description
        {
            get { return _description; }
        }

        LogicSlot _description;
        SlotValueType _valueType;
        VisualElement _control;
        VisualElement _container;
        EdgeControl _edgeControl;

        public PortInputView(LogicSlot description)
        {
            this.LoadAndAddStyleSheet("Styles/PortInputView");
            pickingMode = PickingMode.Ignore;
            ClearClassList();
            _description = description;
            _valueType = description.ValueType;
            AddToClassList("type" + _valueType);

            _edgeControl = new EdgeControl
            {
                @from = new Vector2(212f - 21f, 11.5f),
                to = new Vector2(212f, 11.5f),
                edgeWidth = 2,
                pickingMode = PickingMode.Ignore
            };
            Add(_edgeControl);

            _container = new VisualElement { name = "container" };
            {
                _control = this.Description.InstantiateControl();
                if (_control != null)
                    _container.Add(_control);

                var slotElement = new VisualElement { name = "slot" };
                {
                    slotElement.Add(new VisualElement { name = "dot" });
                }
                _container.Add(slotElement);
            }
            Add(_container);

            _container.visible = _edgeControl.visible = _control != null;
        }

        private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
        {
            Color colorValue;

            if (e.customStyle.TryGetValue(k_EdgeColorProperty, out colorValue))
                _edgeColor = colorValue;
            
            _edgeControl.UpdateLayout();
            _edgeControl.inputColor = edgeColor;
            _edgeControl.outputColor = edgeColor;
        }

        public void UpdateSlot(LogicSlot newLogicSlot)
        {
            _description = newLogicSlot;
            Recreate();
        }

        public void UpdateSlotType()
        {
            if (Description.ValueType != _valueType)
                Recreate();
        }

        void Recreate()
        {
            RemoveFromClassList("type" + _valueType);
            _valueType = Description.ValueType;
            AddToClassList("type" + _valueType);
            if (_control != null)
            {
                var disposable = _control as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
                _container.Remove(_control);
            }
            _control = Description.InstantiateControl();
            if (_control != null)
                _container.Insert(0, _control);

            _container.visible = _edgeControl.visible = _control != null;
        }

        public void Dispose()
        {
            var disposable = _control as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
