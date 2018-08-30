using System;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GeoTetra.GTGenericGraph.Slots
{
	public static class GenericNodeAdaptors
	{
		public static void FloatToFloat(this NodeAdapter nodeAdapter, float a, float b)
		{ }
		
		public static void BoolToBool(this NodeAdapter nodeAdapter, float a, float b)
		{ }
	}
}
