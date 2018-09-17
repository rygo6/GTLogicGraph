using System.Reflection;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
	public interface INodeControlAttribute
	{
		VisualElement InstantiateControl(NodeEditor nodeEditor, PropertyInfo propertyInfo);
	}
}
