using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.AccessControl;
using System.Threading;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    public class GraphLogicData : ScriptableObject
    {
        [SerializeField] private GraphData _graphData;
        
        public void Initialize(GraphData graphData)
        {
            _graphData = graphData;
        }

        public void LoadLogicNodeGraph(List<LogicNode> nodes, List<LogicNode> inputNodes, List<LogicNode> outputNodes)
        {
            if (_graphData == null)
                return;

            nodes.Clear();
            for (int i = 0; i < _graphData.SerializedNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(_graphData.SerializedNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    nodes.Add(node);
                }
            }

            inputNodes.Clear();
            Debug.Log(_graphData.SerializedInputNodes.Count);
            for (int i = 0; i < _graphData.SerializedInputNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(_graphData.SerializedInputNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    inputNodes.Add(node);
                }
            }

            outputNodes.Clear();
            for (int i = 0; i < _graphData.SerializedOutputNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(_graphData.SerializedOutputNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    outputNodes.Add(node);
                }
            }

            foreach (var serializedEdge in _graphData.SerializedEdges)
            {
                LogicNode sourceNode = FindNodeByGuid(serializedEdge.SourceNodeGuid, nodes, inputNodes, outputNodes);
                if (sourceNode == null)
                {
                    Debug.LogWarning("source node is null for edge " + serializedEdge);
                    return;
                }
                
                LogicNode targetNode = FindNodeByGuid(serializedEdge.TargetNodeGuid, nodes, inputNodes, outputNodes);
                if (targetNode == null)
                {
                    Debug.LogWarning("target node is null for edge " + serializedEdge);
                    return;
                }

                MethodInfo targetMethodInfo = MethodInfoByName(targetNode, serializedEdge.TargetMemberName);
                SubscribeToEventByName(sourceNode, serializedEdge.SourceMemberName, targetNode, targetMethodInfo);
            }
        }

        private MethodInfo MethodInfoByName(LogicNode node, string memberName)
        {
            var methods = node.GetType()
                .GetMethods(BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance);
            foreach (MethodInfo method in methods)
            {
                //TODO is nodeport attribute necessary? Maybe just search by name?
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

        private void SubscribeToEventByName(
            LogicNode sourceNode, 
            string memberName, 
            LogicNode targetNode,
            MethodInfo targetMethodInfo)
        {
            var events = sourceNode.GetType()
                .GetEvents(BindingFlags.Public |
                           BindingFlags.NonPublic |
                           BindingFlags.Instance);
            foreach (EventInfo eventInfo in events)
            {
                //TODO is nodeport attribute necessary? Maybe just search by name?
                var attrs = eventInfo.GetCustomAttributes(typeof(NodePortAttribute), false) as NodePortAttribute[];
                for (int i = 0; i < attrs.Length; ++i)
                {
                    if (eventInfo.Name == memberName)
                    {
                        Type type = eventInfo.EventHandlerType;
                        Delegate handler = Delegate.CreateDelegate(type, targetNode, targetMethodInfo);
                        eventInfo.AddEventHandler(sourceNode, handler);
                        return;
                    }
                }
            }
        }

        private LogicNode FindNodeByGuid(string guid, params List<LogicNode>[] lists)
        {
            foreach (var list in lists)
            {
                LogicNode node = list.Find(n => n.NodeGuid == guid);
                if (node != null) return node;
            }

            return null;
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