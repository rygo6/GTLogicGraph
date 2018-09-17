using System;
using System.Reflection;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class InputAttribute : Attribute
    {
        public abstract void HookUpMethodInvoke(LogicNode node, MethodInfo method, GraphInput graphInput);
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class FloatInputAttribute : InputAttribute
    {
        private float _priorFloatValue;

        public override void HookUpMethodInvoke(LogicNode node, MethodInfo method, GraphInput graphInput)
        {
            graphInput.OnValidate = () => OnValidate(node, method, graphInput);
        }

        private void OnValidate(LogicNode node, MethodInfo method, GraphInput graphInput)
        {
            Debug.Log(graphInput.FloatValue + " != " + _priorFloatValue);
            if (graphInput.FloatValue != _priorFloatValue)
            {
                method.Invoke(node, new object[] {graphInput.FloatValue});
                _priorFloatValue = graphInput.FloatValue;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Event)]
    public class OutputAttribute : Attribute
    {
    }
}