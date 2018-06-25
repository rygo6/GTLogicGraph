using System;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace GeoTetra.GTGenericGraph
{
    public class GenericPortInputView : GraphElement, IDisposable
    {
        const string EdgeColorProperty = "edge-color";

        StyleValue<Color> _edgeColor;

        public Color EdgeColor
        {
            get { return _edgeColor.GetSpecifiedValueOrDefault(Color.red); }
        }

        public GenericSlot Slot
        {
            get { return _slot; }
        }

        GenericSlot _slot;
        ConcreteSlotValueType _slotType;
        VisualElement _control;
        VisualElement _container;
        EdgeControl _edgeControl;

        public GenericPortInputView(GenericSlot slot)
        {
            AddStyleSheetPath("Styles/GenericPortInputView");
            pickingMode = PickingMode.Ignore;
            ClearClassList();
            _slot = slot;
            _slotType = slot.concreteValueType;
            AddToClassList("type" + _slotType);

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
                _control = _slot.InstantiateControl();
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

        protected override void OnStyleResolved(ICustomStyle styles)
        {
            base.OnStyleResolved(styles);
            styles.ApplyCustomProperty(EdgeColorProperty, ref _edgeColor);
            _edgeControl.UpdateLayout();
            _edgeControl.inputColor = EdgeColor;
            _edgeControl.outputColor = EdgeColor;
        }

        public void UpdateSlot(GenericSlot newSlot)
        {
            _slot = newSlot;
            Recreate();
        }

        public void UpdateSlotType()
        {
            if (Slot.concreteValueType != _slotType)
                Recreate();
        }

        void Recreate()
        {
            RemoveFromClassList("type" + _slotType);
            _slotType = Slot.concreteValueType;
            AddToClassList("type" + _slotType);
            if (_control != null)
            {
                var disposable = _control as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
                _container.Remove(_control);
            }
            _control = Slot.InstantiateControl();
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
