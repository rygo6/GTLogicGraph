using System.Reflection;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTLogicGraph
{
	public interface INodeControlAttribute
	{
		VisualElement InstantiateControl(LogicNodeEditor logicNodeEditor, PropertyInfo propertyInfo);
	}
}
