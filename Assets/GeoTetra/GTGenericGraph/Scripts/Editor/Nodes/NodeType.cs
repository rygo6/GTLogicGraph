using System;

namespace GeoTetra.GTGenericGraph
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NodeType : Attribute
	{
		public readonly string Name;
		public NodeType(string name) { this.Name = name; }
	}
}
