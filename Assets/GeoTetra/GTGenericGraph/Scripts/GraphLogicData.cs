using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            get
            {
                if (_inputNodes.Count == 0)
                    LoadLogicNodeGraph();
                return _inputNodes;
            }
        }

        public List<LogicNode> OutputNodes
        {
            get
            {
                if (_outputNodes.Count == 0)
                    LoadLogicNodeGraph();
                return _outputNodes;
            }
        }

        private void OnDisable()
        {
            Debug.Log("GraphLogicData OnDisable");
            //prior instances must be cleared so when this object is reloaded
            //the instances are reloaded, because they aren't saved by serialization
            _outputNodes.Clear();
            _inputNodes.Clear();
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
                LogicNode sourceNode = FindNodeByGuid(serializedEdge.SourceNodeGuid);
                if (sourceNode == null)
                {
                    Debug.LogWarning("source node is null for edge " + serializedEdge);
                    return;
                }
                
                LogicNode targetNode = FindNodeByGuid(serializedEdge.TargetNodeGuid);
                if (targetNode == null)
                {
                    Debug.LogWarning("target node is null for edge " + serializedEdge);
                    return;
                }

                MethodInfo targetMethodInfo = MethodInfoByIndex(targetNode, serializedEdge.TargetIndex);
                SubscribeToEventByIndex(sourceNode, serializedEdge.SourceIndex, targetNode, targetMethodInfo);
            }
        }

        private MethodInfo MethodInfoByIndex(LogicNode node, string memberName)
        {
            var methods = node.GetType()
                .GetMethods(BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                var attrs = method.GetCustomAttributes(typeof(NodePortAttribute), false) as NodePortAttribute[];
                for (int i = 0; i < attrs.Length; ++i)
                {
                    if (method.Name == memberName)
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        private void SubscribeToEventByIndex(LogicNode sourceNode, string memberName, LogicNode targetNode,
            MethodInfo targetMethodInfo)
        {
            var events = sourceNode.GetType()
                .GetEvents(BindingFlags.Public |
                           BindingFlags.NonPublic |
                           BindingFlags.Instance);
            foreach (EventInfo eventInfo in events)
            {
                var attrs = eventInfo.GetCustomAttributes(typeof(NodePortAttribute), false) as NodePortAttribute[];
                for (int i = 0; i < attrs.Length; ++i)
                {
                    if (eventInfo.Name == memberName)
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
                        if (type.Name == serializedNode.NodeType)
                        {
                            return JsonUtility.FromJson(serializedNode.JSON, type) as LogicNode;
                        }
                    }
                }
            }

            return null;
        }
    }
}