using System.Reflection;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph
{
	public interface IGenericControlAttribute
	{
		VisualElement InstantiateControl(AbstractGenericNode node, PropertyInfo propertyInfo);
	}
}
