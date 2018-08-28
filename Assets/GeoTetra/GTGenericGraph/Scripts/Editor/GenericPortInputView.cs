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

        public GenericPortDescription PortDescription
        {
            get { return _portDescription; }
        }

        GenericPortDescription _portDescription;
        ConcreteSlotValueType _slotType;
        VisualElement _control;
        VisualElement _container;
        EdgeControl _edgeControl;

        public GenericPortInputView(GenericPortDescription portDescription)
        {
            AddStyleSheetPath("Styles/GenericPortInputView");
            pickingMode = PickingMode.Ignore;
            ClearClassList();
            _portDescription = portDescription;
            _slotType = portDescription.concreteValueType;
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

        public void Dispose()
        {
            var disposable = _control as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }
    }
}
