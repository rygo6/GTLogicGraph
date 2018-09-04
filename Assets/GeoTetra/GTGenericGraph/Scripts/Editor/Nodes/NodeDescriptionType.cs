using System;

namespace GeoTetra.GTGenericGraph
{
	[AttributeUsage(AttributeTargets.Class)]
	public class NodeDescriptionType : Attribute
	{
		public readonly string Name;
		public NodeDescriptionType(string name) { this.Name = name; }
	}
}
