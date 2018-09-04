using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
    public class GraphLogicData : ScriptableObject
    {
        [SerializeField] private GraphData _graphData;

        private List<LogicNode> _inputNodes = new List<LogicNode>();
        private List<LogicNode> _outputNodes = new List<LogicNode>();
        private List<LogicNode> _nodes = new List<LogicNode>();

        private void Awake()
        {
            Debug.Log(name + " Awake");
            LoadLogiceNodeGraph();
        }
        
        private void OnEnable()
        {
            Debug.Log(name + " OnEnable");
            if (_nodes.Count > 0)
            Debug.Log(_nodes[0].GetType());
        }
        
        private void OnDisable()
        {
            Debug.Log(name + " OnDisable");
        }

        public void Initialize(GraphData graphData)
        {
            _graphData = graphData;
            LoadLogiceNodeGraph();
        }

        private void LoadLogiceNodeGraph()
        {
            if (_graphData == null)
                return;
            
            for (int i = 0; i < _graphData.SerializedNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(_graphData.SerializedNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    _nodes.Add(node);
                }
            }

            for (int i = 0; i < _graphData.SerializedInputNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(_graphData.SerializedInputNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    _inputNodes.Add(node);
                }
            }

            for (int i = 0; i < _graphData.SerializedOutputNodes.Count; ++i)
            {
                LogicNode node = CreateLogicNodeFromSerializedNode(_graphData.SerializedOutputNodes[i]);
                Debug.Log("Adding node " + node);
                if (node != null)
                {
                    _outputNodes.Add(node);
                }
            }

            Debug.Log(_nodes[0].GetType());
            
            for (int i = 0; i < _graphData.SerializedEdges.Count; ++i)
            {
            }
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