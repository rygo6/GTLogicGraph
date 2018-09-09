using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.Events;

namespace GeoTetra.GTGenericGraph
{
    public class GraphLogic : MonoBehaviour
    {
        [SerializeField] private GraphLogicData _graphLogicData;

        [SerializeField] private List<GraphInput> _inputs;

        [SerializeField] private List<GraphOutput> _outputs;

        private void OnValidate()
        {
            for (int i = 0; i < _inputs.Count; ++i)
            {
                if (_inputs[i].OnValidate == null) HookUpInput(_inputs[i]);
                if (_inputs[i].OnValidate != null) _inputs[i].OnValidate();
            }
        }

        [ContextMenu("LoadInputs")]
        private void LoadInputs()
        {
            _inputs.Clear();
            foreach (var node in _graphLogicData.InputNodes)
            {
                var methods = node.GetType()
                    .GetMethods(BindingFlags.Public | 
                                BindingFlags.NonPublic |
                                BindingFlags.Instance);
                foreach (MethodInfo method in methods)
                {
                    var attrs = method.GetCustomAttributes(typeof(InputAttribute), false) as InputAttribute[];
                    for (int i = 0; i < attrs.Length; ++i)
                    {
                        GraphInput input = new GraphInput
                        {
                            Id = attrs[0].Id,
                            NodeGuid = node.NodeGuid,
                            Name = method.Name
                        };

                        _inputs.Add(input);
                    }
                }
            }
        }
        
        [ContextMenu("LoadOutputs")]
        private void LoadOutputs()
        {
            _outputs.Clear();
            foreach (var node in _graphLogicData.OutputNodes)
            {
                Debug.Log(node.GetType());
                var events = node.GetType()
                    .GetEvents(BindingFlags.Public | 
                               BindingFlags.NonPublic | 
                               BindingFlags.Instance);
                foreach (EventInfo eventInfo in events)
                {
                    var attrs = eventInfo.GetCustomAttributes(typeof(OutputAttribute), false) as OutputAttribute[];
                    for (int i = 0; i < attrs.Length; ++i)
                    {
                        GraphOutput output = new GraphOutput
                        {
                            Id = attrs[0].Id,
                            NodeGuid = node.NodeGuid,
                            Name = eventInfo.Name
                        };

                        MethodInfo method = typeof(GraphOutput).GetMethod("RaiseUpdated",
                            BindingFlags.Public | BindingFlags.Instance);

                        Type type = eventInfo.EventHandlerType;
                        Delegate handler = Delegate.CreateDelegate(type, output, method);
                        eventInfo.AddEventHandler(node, handler);

                        _outputs.Add(output);
                    }
                }
            }
        }

        private void HookUpInput(GraphInput graphInput)
        {
            LogicNode node = _graphLogicData.InputNodes.Find(n => n.NodeGuid == graphInput.NodeGuid);
            var methods = node.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                var attrs = method.GetCustomAttributes(typeof(InputAttribute), false) as InputAttribute[];
                for (int i = 0; i < attrs.Length; ++i)
                {
                    if (attrs[0].Id == graphInput.Id)
                    {
                        attrs[0].HookUpMethodInvoke(node, method, graphInput);
                    }
                }
            }
        }
    }

    [Serializable]
    public class GraphInput
    {
        public string Name;
        public int Id;
        public string NodeGuid;
        public float FloatValue;
        public Action OnValidate;
    }

    [Serializable]
    public class GraphOutput
    {
        public string Name;
        public int Id;
        public string NodeGuid;
        public FloatUnityEvent Updated;

        public void RaiseUpdated(float value)
        {
            Debug.Log("RaiseUpdated " + value);
            Updated.Invoke(value);
        }
    }
    

    [Serializable]
    public class FloatUnityEvent : UnityEvent<float>
    {
    }
}