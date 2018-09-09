using System;
using System.Reflection;

namespace GeoTetra.GTGenericGraph
{
	[AttributeUsage(AttributeTargets.Method)]
	public abstract class InputAttribute : Attribute
	{
		public readonly int Id;
		public InputAttribute(int id) { this.Id = id; }
		public abstract void HookUpMethodInvoke(LogicNode node, MethodInfo method, GraphInput graphInput);
	}
	
	[AttributeUsage(AttributeTargets.Method)]
	public class FloatInputAttribute : InputAttribute
	{
		public FloatInputAttribute(int id) : base(id)
		{ }
		
		public override void HookUpMethodInvoke(LogicNode node, MethodInfo method, GraphInput graphInput)
		{
			graphInput.OnValidate = () => method.Invoke(node, new object[] {graphInput.FloatValue});
		}
	}
	
	[AttributeUsage(AttributeTargets.Event)]
	public class OutputAttribute : Attribute
	{
		public readonly int Id;
		public OutputAttribute(int id) { this.Id = id; }
//		public abstract void HookUpMethodInvoke(LogicNode node, MethodInfo method, GraphInput graphInput);
	}
}
