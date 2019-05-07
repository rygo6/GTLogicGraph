using System.Reflection;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph
{
	public interface INodeControlAttribute
	{
		VisualElement InstantiateControl(AbstractLogicNodeEditor logicNodeEditor, PropertyInfo propertyInfo);
	}
}
