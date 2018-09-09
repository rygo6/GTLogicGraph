using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.AccessControl;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    public class GraphLogicData : ScriptableObject
    {
        [SerializeField] private GraphData _graphData;

        private List<LogicNode> _inputNodes = new List<LogicNode>();
        private List<LogicNode> _outputNodes = new List<LogicNode>();
        private List<LogicNode> _nodes = new List<LogicNode>();

        public List<LogicNode> InputNodes
        {
            get { return _inputNodes; }
        }

        public List<LogicNode> OutputNodes
        {
            get { return _outputNodes; }
        }

        private void Awake()
        {
            Debug.Log(name + " Awake");
        }

        private void OnEnable()
        {
            LoadLogicNodeGraph();
            Debug.Log(name + " OnEnable");
        }

        private void OnDisable()
        {
            Debug.Log(name + " OnDisable");
        }

        public void Initialize(GraphData graphData)
        {
            _graphData = graphData;
        }

        [ContextMenu("LoadLogicNodeGraph")]
        private void LoadLogicNodeGraph()
        {
            if (_graphData == null)
                return;

            _nodes.Clear();
            for (int i = 0; i < _graphData.SerializedNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(_graphData.SerializedNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    _nodes.Add(node);
                }
            }

            _inputNodes.Clear();
            for (int i = 0; i < _graphData.SerializedInputNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(_graphData.SerializedInputNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    _inputNodes.Add(node);
                }
            }

            _outputNodes.Clear();
            for (int i = 0; i < _graphData.SerializedOutputNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(_graphData.SerializedOutputNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    _outputNodes.Add(node);
                }
            }

            foreach (var serializedEdge in _graphData.SerializedEdges)
            {
                Debug.Log(serializedEdge);
                LogicNode sourceNode = FindNodeByGuid(serializedEdge.Source);
                Debug.Log(sourceNode);
                LogicNode targetNode = FindNodeByGuid(serializedEdge.Target);
                Debug.Log(targetNode);
                MethodInfo targetMethodInfo = MethodInfoByIndex(targetNode, serializedEdge.TargetIndex);
                Debug.Log(targetMethodInfo);
                SubscribeToEventByIndex(sourceNode, serializedEdge.SourceIndex, targetNode, targetMethodInfo);
            }
        }

        private MethodInfo MethodInfoByIndex(LogicNode node, int index)
        {
            var methods = node.GetType()
                .GetMethods(BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                var attrs = method.GetCustomAttributes(typeof(PortIndexAttribute), false) as PortIndexAttribute[];
                for (int i = 0; i < attrs.Length; ++i)
                {
                    if (attrs[i].Id == index)
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        private void SubscribeToEventByIndex(LogicNode sourceNode, int sourceIndex, LogicNode targetNode, MethodInfo targetMethodInfo)
        {
            var events = sourceNode.GetType()
                .GetEvents(BindingFlags.Public |
                           BindingFlags.NonPublic |
                           BindingFlags.Instance);
            foreach (EventInfo eventInfo in events)
            {
                var attrs = eventInfo.GetCustomAttributes(typeof(PortIndexAttribute), false) as PortIndexAttribute[];
                for (int i = 0; i < attrs.Length; ++i)
                {
                    if (attrs[i].Id == sourceIndex)
                    {
                        MethodInfo method = typeof(GraphOutput).GetMethod("RaiseUpdated",
                            BindingFlags.Public | BindingFlags.Instance);

                        Type type = eventInfo.EventHandlerType;
                        Delegate handler = Delegate.CreateDelegate(type, targetNode, targetMethodInfo);
                        eventInfo.AddEventHandler(sourceNode, handler);
                        return;
                    }
                }
            }
        }

        private LogicNode FindNodeByGuid(string guid)
        {
            LogicNode node = _nodes.Find(n => n.NodeGuid == guid);
            if (node != null) return node;
            node = _inputNodes.Find(n => n.NodeGuid == guid);
            if (node != null) return node;
            node = _outputNodes.Find(n => n.NodeGuid == guid);
            return node;
        }

        private LogicNode CreateLogicNodeFromSerializedNode(SerializedNode serializedNode)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(LogicNode)))
                    {
                        var attrs = type.GetCustomAttributes(typeof(LogicNodeType), false) as LogicNodeType[];
                        if (attrs != null && attrs.Length > 0)
                        {
                            if (attrs[0].Name == serializedNode.NodeType)
                            {
                                return JsonUtility.FromJson(serializedNode.JSON, type) as LogicNode;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}