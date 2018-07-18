using UnityEditor.ShaderGraph;
using System;
using UnityEditor.Graphing;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
	public interface IGenericProperty
	{
		string displayName { get; set; }
		GenericPropertyType propertyType { get; } // is this necessary with GetPropertyType?
		Guid guid { get; }
		Vector4 defaultValue { get; }
		Type GetPropertyType();
		INode ToConcreteNode();
	}
}