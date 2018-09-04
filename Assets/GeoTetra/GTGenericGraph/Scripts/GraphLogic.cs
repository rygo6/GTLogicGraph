using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    public class GraphLogic : MonoBehaviour
    {
        [SerializeField] private GraphLogicData _graphLogicData;

        public event Action<float> Output;

        [SerializeField] private List<GraphInput> _inputs;

        private void OnValidate()
        {
            for (int i = 0; i < _inputs.Count; ++i)
            {
                _inputs[i].OnValidate();
            }
        }

        [ContextMenu("LoadInputs")]
        private void LoadInputs()
        {
            _inputs.Clear();
            LogicNode node = _graphLogicData.InputNodes[0];
            Debug.Log(node.GetType());
            var methods = node.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                var attrs = method.GetCustomAttributes(typeof(InputAttribute), false) as InputAttribute[];
                if (attrs != null && attrs.Length > 0)
                {
                    GraphInput input = new GraphInput
                    {
                        NodeGuid = node.NodeGuid,
                        Name = method.Name
                    };
                    input.OnValidate = () => method.Invoke(node, new object[] {input.FloatValue});

                    _inputs.Add(input);
                }
            }
        }

//        private Type TypeToGraphInput(Type type)
//        {
//            switch (type)
//            {
//                case FloatInput:
//                    return null;
//                    yield break;
//            }
//        }
    }

    [Serializable]
    public class GraphInput
    {
        public string NodeGuid;
        public string Name;
        public float FloatValue;
        public Action OnValidate;
    }
}