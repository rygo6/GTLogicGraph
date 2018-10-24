using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace GeoTetra.GTLogicGraph
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class InputAttribute : Attribute
    {
        public abstract void HookUpMethodInvoke(LogicNode node, MethodInfo method, GraphInput graphInput);

        public abstract Type InputType();
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class Vector1InputAttribute : InputAttribute
    {
        private float _priorFloatValueX;

        public override void HookUpMethodInvoke(LogicNode node, MethodInfo method, GraphInput graphInput)
        {
            graphInput.OnValidate = () => OnValidate(node, method, graphInput);
        }

        public override Type InputType()
        {
            return typeof(float);
        }
        
        private void OnValidate(LogicNode node, MethodInfo method, GraphInput graphInput)
        {
            if (!Mathf.Approximately(graphInput.FloatValueX, _priorFloatValueX))
            {
                method.Invoke(node, new object[] {graphInput.FloatValueX});
                _priorFloatValueX = graphInput.FloatValueX;
            }
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class Vector3InputAttribute : InputAttribute
    {
        private float _priorFloatValueX;
        private float _priorFloatValueY;
        private float _priorFloatValueZ;

        public override void HookUpMethodInvoke(LogicNode node, MethodInfo method, GraphInput graphInput)
        {
            graphInput.OnValidate = () => OnValidate(node, method, graphInput);
        }

        public override Type InputType()
        {
            return typeof(float);
        }
        
        private void OnValidate(LogicNode node, MethodInfo method, GraphInput graphInput)
        {
            if (!Mathf.Approximately(graphInput.FloatValueX, _priorFloatValueX) ||
                !Mathf.Approximately(graphInput.FloatValueY, _priorFloatValueY) ||
                !Mathf.Approximately(graphInput.FloatValueZ, _priorFloatValueZ))
            {
                method.Invoke(node, new object[] {new Vector3(graphInput.FloatValueX, graphInput.FloatValueY, graphInput.FloatValueZ)});
                _priorFloatValueX = graphInput.FloatValueX;
                _priorFloatValueY = graphInput.FloatValueY;
                _priorFloatValueZ = graphInput.FloatValueZ;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Event)]
    public class OutputAttribute : Attribute
    {
    }
}