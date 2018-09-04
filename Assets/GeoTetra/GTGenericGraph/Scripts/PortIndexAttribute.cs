using System;

namespace GeoTetra.GTGenericGraph
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Event)]
	public class PortIndexAttribute : Attribute
	{
		public readonly int Id;
		public PortIndexAttribute(int id) { this.Id = id; }
	}
}
