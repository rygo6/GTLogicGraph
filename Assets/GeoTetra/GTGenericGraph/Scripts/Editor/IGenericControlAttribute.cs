using System.Reflection;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
	public interface IGenericControlAttribute
	{
		VisualElement InstantiateControl(NodeEditor nodeEditor, PropertyInfo propertyInfo);
	}
}
