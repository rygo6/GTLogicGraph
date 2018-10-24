using System;

namespace GeoTetra.GTLogicGraph
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NodeEditorType : Attribute
	{
		public readonly Type NodeType;
		public NodeEditorType(Type nodeType) { this.NodeType = nodeType; }
	}
}
