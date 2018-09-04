using System;

namespace GeoTetra.GTGenericGraph
{
	[AttributeUsage(AttributeTargets.Class)]
	public class LogicNodeType : Attribute
	{
		public readonly string Name;
		public LogicNodeType(string name) { this.Name = name; }
	}
}
