using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace GeoTetra.GTGenericGraph
{
    [ExecuteInEditMode]
    public class GraphLogic : MonoBehaviour
    {
        [SerializeField] private GraphLogicData _graphLogicData;

        [SerializeField] private List<GraphInput> _inputs;

        [SerializeField] private List<GraphOutput> _outputs;

        private void OnEnable()
        {
            Debug.Log("GraphLogic OnEnable");

            if (_graphLogicData.InputNodes.Count != 0)
            {
                List<GraphInput> loadedInputs = new List<GraphInput>();
                LoadInputs(loadedInputs);
                Debug.Log(loadedInputs.Count);

                //add nodes
                for (int i = 0; i < loadedInputs.Count; ++i)
                {
                    if (_inputs.Find(n => n.NodeGuid == loadedInputs[i].NodeGuid) == null)
                    {
                        _inputs.Add(loadedInputs[i]);
                    }
                }

                //remove nodes and hook up
                for (int i = _inputs.Count - 1; i > -1; --i)
                {
                    if (loadedInputs.Find(n => n.NodeGuid == _inputs[i].NodeGuid) == null)
                    {
                        _inputs.RemoveAt(i);
                    }
                    else
                    {
                        HookUpInput(_inputs[i]);
                    }
                }
            }

            if (_graphLogicData.OutputNodes.Count != 0)
            {
                List<GraphOutput> loadedOutputs = new List<GraphOutput>();
                LoadOutputs(loadedOutputs);
                Debug.Log(loadedOutputs.Count);

                //add nodes
                for (int i = 0; i < loadedOutputs.Count; ++i)
                {
                    if (_outputs.Find(n => n.NodeGuid == loadedOutputs[i].NodeGuid) == null)
                    {
                        _outputs.Add(loadedOutputs[i]);
                    }
                }

                //remove nodes and hook up
                for (int i = _outputs.Count - 1; i > -1; --i)
                {
                    if (loadedOutputs.Find(n => n.NodeGuid == _outputs[i].NodeGuid) == null)
                    {
                        _outputs.RemoveAt(i);
                    }
                    else
                    {
                        HookUpOutput(_outputs[i]);
                    }
                }
            }
        }

        private void OnValidate()
        {
            for (int i = 0; i < _inputs.Count; ++i)
            {
                if (_inputs[i].OnValidate != null) _inputs[i].OnValidate();
            }
        }

        private void LoadInputs(List<GraphInput> inputs)
        {
            foreach (var node in _graphLogicData.InputNodes)
            {
                var methods = node.GetType().GetMethods(BindingFlags.Public |
                                                        BindingFlags.NonPublic |
                                                        BindingFlags.Instance);
                foreach (MethodInfo method in methods)
                {
                    var attrs = method.GetCustomAttributes(typeof(InputAttribute), false) as InputAttribute[];
                    if (attrs.Length > 0)
                    {
                        GraphInput input = new GraphInput
                        {
                            MemberName = method.Name,
                            NodeGuid = node.NodeGuid,
                            DisplayName = node.DisplayName + " " + method.Name
                        };
                        attrs[0].HookUpMethodInvoke(node, method, input);
                        inputs.Add(input);
                    }
                    else
                    {
                        Debug.LogWarning(method.Name + " on  " + node + "has multiple input attributes.");
                    }
                }
            }
        }
                
        private void HookUpInput(GraphInput graphInput)
        {
            LogicNode node = _graphLogicData.InputNodes.Find(n => n.NodeGuid == graphInput.NodeGuid);
            var methods = node.GetType().GetMethods(BindingFlags.Public |
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                var attrs = method.GetCustomAttributes(typeof(InputAttribute), false) as InputAttribute[];
                if (attrs.Length > 0)
                {
                    if (method.Name == graphInput.MemberName)
                    {
                        attrs[0].HookUpMethodInvoke(node, method, graphInput);
                    }
                }
                else
                {
                    Debug.LogWarning(method.Name + " on  " + node + "has multiple input attributes.");
                }
            }
        }
        
        private void LoadOutputs(List<GraphOutput> outputs)
        {
            foreach (var node in _graphLogicData.OutputNodes)
            {
                var events = node.GetType().GetEvents(BindingFlags.Public |
                                                      BindingFlags.NonPublic |
                                                      BindingFlags.Instance);
                foreach (EventInfo eventInfo in events)
                {
                    var attrs = eventInfo.GetCustomAttributes(typeof(OutputAttribute), false) as OutputAttribute[];
                    if (attrs.Length > 0)
                    {
                        GraphOutput graphOutput = new GraphOutput
                        {
                            MemberName = eventInfo.Name,
                            NodeGuid = node.NodeGuid,
                            DisplayName = node.DisplayName + " " + eventInfo.Name
                        };
                        graphOutput.SubscribeRaiseUpdate(eventInfo, node);
                        outputs.Add(graphOutput);
                    }
                    else
                    {
                        Debug.LogWarning(eventInfo.Name + " on  " + node + "has multiple input attributes.");
                    }
                }
            }
        }

        private void HookUpOutput(GraphOutput graphOutput)
        {
            LogicNode node = _graphLogicData.OutputNodes.Find(n => n.NodeGuid == graphOutput.NodeGuid);
            var events = node.GetType().GetEvents(BindingFlags.Public |
                                                  BindingFlags.NonPublic |
                                                  BindingFlags.Instance);
            foreach (EventInfo eventInfo in events)
            {
                var attrs = eventInfo.GetCustomAttributes(typeof(OutputAttribute), false) as OutputAttribute[];
                if (attrs.Length > 0)
                {
                    if (eventInfo.Name == graphOutput.MemberName)
                    {
                        graphOutput.SubscribeRaiseUpdate(eventInfo, node);
                    }
                }
                else
                {
                    Debug.LogWarning(eventInfo.Name + " on  " + node + "has multiple input attributes.");
                }
            }
        }
    }

    [Serializable]
    public class GraphInput
    {
        public string DisplayName;
        public string MemberName;
        public string NodeGuid;
        public float FloatValue;
        public Action OnValidate;
    }

    [Serializable]
    public class GraphOutput
    {
        public string DisplayName;
        public string MemberName;
        public string NodeGuid;
        public FloatUnityEvent Updated = new FloatUnityEvent();

        private void RaiseUpdated(float value)
        {
            Debug.Log("RaiseUpdated " + value);
            Updated.Invoke(value);
        }

        public void SubscribeRaiseUpdate(EventInfo eventInfo, LogicNode node)
        {
            MethodInfo method = typeof(GraphOutput).GetMethod("RaiseUpdated",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            Type type = eventInfo.EventHandlerType;
            Delegate handler = Delegate.CreateDelegate(type, this, method);
            eventInfo.AddEventHandler(node, handler);
        }
    }


    [Serializable]
    public class FloatUnityEvent : UnityEvent<float>
    {
    }
}