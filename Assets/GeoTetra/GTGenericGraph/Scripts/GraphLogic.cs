using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Events;
using Object = System.Object;

namespace GeoTetra.GTGenericGraph
{
    [ExecuteInEditMode]
    public class GraphLogic : MonoBehaviour
    {
        [SerializeField] private GraphLogicData _graphLogicData;
        [SerializeField] private List<GraphInput> _inputs;
        [SerializeField] private List<GraphOutput> _outputs;

        private List<LogicNode> _inputNodes = new List<LogicNode>();
        private List<LogicNode> _outputNodes = new List<LogicNode>();
        private List<LogicNode> _nodes = new List<LogicNode>();

        public List<GraphInput> Inputs
        {
            get { return _inputs; }
        }

        public List<GraphOutput> Outputs
        {
            get { return _outputs; }
        }

        private void Awake()
        {
            Debug.Log("Start");
        }

        public void OnEnable()
        {
            Debug.Log("OnEnable");
            _graphLogicData.LoadLogicNodeGraph(_nodes, _inputNodes, _outputNodes);
            UpdateInputsAndOutputs();
        }

        private void Reset()
        {
            _inputNodes.Clear();
            _outputNodes.Clear();
            _graphLogicData = null;
        }

        private void OnValidate()
        {
            Debug.Log("OnValidate");

            if (_graphLogicData != null)
            {
                _graphLogicData.LoadLogicNodeGraph(_nodes, _inputNodes, _outputNodes);
                UpdateInputsAndOutputs();
            }

            for (int i = 0; i < Inputs.Count; ++i)
            {
                if (Inputs[i].OnValidate != null) Inputs[i].OnValidate();
            }
        }

        private void UpdateInputsAndOutputs()
        {
            Debug.Log("GraphLogic OnEnable");

            if (_inputNodes.Count != 0)
            {
                List<GraphInput> loadedInputs = new List<GraphInput>();
                LoadInputs(loadedInputs);

                //add nodes
                for (int i = 0; i < loadedInputs.Count; ++i)
                {
                    if (Inputs.Find(n => n.MemberName == loadedInputs[i].MemberName &&
                                         n.NodeGuid == loadedInputs[i].NodeGuid) == null)
                    {
                        Inputs.Add(loadedInputs[i]);
                    }
                }

                //remove nodes and hook up
                for (int i = Inputs.Count - 1; i > -1; --i)
                {
                    if (loadedInputs.Find(n => n.MemberName == Inputs[i].MemberName &&
                                               n.NodeGuid == Inputs[i].NodeGuid) == null)
                    {
                        Inputs.RemoveAt(i);
                    }
                    else
                    {
                        HookUpInput(Inputs[i]);
                    }
                }
            }

            if (_outputNodes.Count != 0)
            {
                List<GraphOutput> loadedOutputs = new List<GraphOutput>();
                LoadOutputs(loadedOutputs);

                //add nodes
                for (int i = 0; i < loadedOutputs.Count; ++i)
                {
                    if (Outputs.Find(n => n.MemberName == loadedOutputs[i].MemberName &&
                                          n.NodeGuid == loadedOutputs[i].NodeGuid) == null)
                    {
                        Outputs.Add(loadedOutputs[i]);
                    }
                }

                //remove nodes and hook up
                for (int i = Outputs.Count - 1; i > -1; --i)
                {
                    if (loadedOutputs.Find(n => n.MemberName == Outputs[i].MemberName &&
                                                n.NodeGuid == Outputs[i].NodeGuid) == null)
                    {
                        Outputs.RemoveAt(i);
                    }
                    else
                    {
                        HookUpOutput(Outputs[i]);
                    }
                }
            }
        }

        private void LoadInputs(List<GraphInput> inputs)
        {
            foreach (var node in _inputNodes)
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
                            DisplayName = node.DisplayName + " " + method.Name,
                            InputType = attrs[0].InputType()
                        };
                        inputs.Add(input);
                        if (attrs.Length > 1)
                            Debug.LogWarning(method.Name + " on  " + node + "has multiple input attributes.");
                    }
                }
            }
        }

        private void HookUpInput(GraphInput graphInput)
        {
            LogicNode node = _inputNodes.Find(n => n.NodeGuid == graphInput.NodeGuid);
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
                        graphInput.InputType = attrs[0].InputType();
                        attrs[0].HookUpMethodInvoke(node, method, graphInput);
                    }

                    if (attrs.Length > 1)
                        Debug.LogWarning(method.Name + " on  " + node + "has multiple input attributes.");
                }
            }
        }

        private void LoadOutputs(List<GraphOutput> outputs)
        {
            foreach (var node in _outputNodes)
            {
                var events = node.GetType().GetEvents(BindingFlags.Public |
                                                      BindingFlags.NonPublic |
                                                      BindingFlags.Instance);
                foreach (EventInfo eventInfo in events)
                {
                    var attrs = eventInfo.GetCustomAttributes(typeof(OutputAttribute), false) as OutputAttribute[];
                    if (attrs.Length > 0)
                    {
                        Type[] types = eventInfo.EventHandlerType.GetGenericArguments();
                        if (types.Length > 1)
                        {
                            Debug.LogError(
                                eventInfo + " has more than one generic type, only one currently supported.");
                        }
                        else if (types.Length == 0)
                        {
                            Debug.LogError(eventInfo + " has more than no generic types.");
                        }

                        GraphOutput graphOutput = new GraphOutput();
                        graphOutput.MemberName = eventInfo.Name;
                        graphOutput.NodeGuid = node.NodeGuid;
                        graphOutput.DisplayName = node.DisplayName + " " + eventInfo.Name;
                        graphOutput.OutputType = types[0];
                        outputs.Add(graphOutput);

                        if (attrs.Length > 1)
                            Debug.LogWarning(eventInfo.Name + " on  " + node + "has multiple input attributes.");
                    }
                }
            }
        }

        private void HookUpOutput(GraphOutput graphOutput)
        {
            LogicNode node = _outputNodes.Find(n => n.NodeGuid == graphOutput.NodeGuid);
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
                        Type[] types = eventInfo.EventHandlerType.GetGenericArguments();
                        graphOutput.OutputType = types[0];
                        graphOutput.SubscribeRaiseUpdate(eventInfo, node);
                    }

                    if (attrs.Length > 1)
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
        public float FloatValueX;
        public float FloatValueY;
        public float FloatValueZ;
        public float FloatValueW;
        public Type InputType;
        public Component ComponentValue;
        public Action OnValidate;
    }


    [Serializable]
    public class GraphOutput
    {
        [SerializeField] private FloatUnityEvent _updatedFloat = new FloatUnityEvent();

        [SerializeField] private Vector3UnityEvent _updatedVector3 = new Vector3UnityEvent();

        [SerializeField] private ObjectUnityEvent _updatedObject = new ObjectUnityEvent();

        public string DisplayName;
        public string MemberName;
        public string NodeGuid;
        public Type OutputType;
        public event Action<ObjectEvent> Updated;

        private void RaiseUpdatedFloat(float value)
        {
            Debug.Log("RaiseUpdatedFloat " + value);
            _updatedFloat.Invoke(value);
            ObjectEvent objectEvent = new ObjectEvent(value, OutputType);
            if (Updated != null) Updated(objectEvent);
        }

        private void RaiseUpdatedVector3(Vector3 value)
        {
            Debug.Log("RaiseUpdatedVector3 " + value);
            _updatedVector3.Invoke(value);
            ObjectEvent objectEvent = new ObjectEvent(value, OutputType);
            if (Updated != null) Updated(objectEvent);
        }

        private void RaiseUpdatedObject(Object value)
        {
            Debug.Log("RaiseUpdatedObject " + value);
            ObjectEvent objectEvent = new ObjectEvent(value, OutputType);
            _updatedObject.Invoke(objectEvent);
            if (Updated != null) Updated(objectEvent);
        }

        public void SubscribeRaiseUpdate(EventInfo eventInfo, LogicNode node)
        {
            Type type = eventInfo.EventHandlerType;
            Type[] passingTypes = eventInfo.EventHandlerType.GetGenericArguments();
            string methodName = "";
            if (passingTypes.Length == 0)
            {
                Debug.LogError(eventInfo + " has no passing type.");
            }
            else if (passingTypes[0] == typeof(Single))
            {
                methodName = "RaiseUpdatedFloat";
            }
            else if (passingTypes[0] == typeof(Vector3))
            {
                methodName = "RaiseUpdatedVector3";
            }
            else
            {
                methodName = "RaiseUpdatedObject";
            }

            MethodInfo method = typeof(GraphOutput).GetMethod(methodName,
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.NonPublic);
            Delegate handler = Delegate.CreateDelegate(type, this, method);
            eventInfo.AddEventHandler(node, handler);
        }
    }

    [Serializable]
    public struct ObjectEvent
    {
        public readonly Object ObjectValue;
        public readonly Type ObjectType;

        public ObjectEvent(Object objectValue, Type objectType)
        {
            ObjectValue = objectValue;
            ObjectType = objectType;
        }

        public T TypedValue<T>()
        {
            return (T) Convert.ChangeType(ObjectValue, ObjectType);
        }
    }

    [Serializable]
    public class ObjectUnityEvent : UnityEvent<ObjectEvent>
    {
    }

    [Serializable]
    public class FloatUnityEvent : UnityEvent<float>
    {
    }

    [Serializable]
    public class Vector3UnityEvent : UnityEvent<Vector3>
    {
    }
}