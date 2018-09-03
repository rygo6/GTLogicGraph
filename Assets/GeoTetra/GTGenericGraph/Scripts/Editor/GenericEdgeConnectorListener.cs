using UnityEditor.Experimental.UIElements.GraphView;
using UnityEditor.Graphs;
using UnityEngine;
using Edge = UnityEditor.Experimental.UIElements.GraphView.Edge;
using UnityEditor.ShaderGraph.Drawing;

namespace GeoTetra.GTGenericGraph
{
    public class GenericEdgeConnectorListener : IEdgeConnectorListener
    {
        private readonly GenericGraphEditorView _genericGraphEditorView;
        private readonly GenericSearchWindowProvider _searchWindowProvider;

        public GenericEdgeConnectorListener(GenericGraphEditorView genericGraphEditorView, GenericSearchWindowProvider searchWindowProvider)
        {
            _genericGraphEditorView = genericGraphEditorView;
            _searchWindowProvider = searchWindowProvider;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            var draggedPort = (edge.output != null ? edge.output.edgeConnector.edgeDragHelper.draggedPort : null) ??
                              (edge.input != null ? edge.input.edgeConnector.edgeDragHelper.draggedPort : null);
            _searchWindowProvider.ConnectedPortView = (PortView) draggedPort;
            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                _searchWindowProvider);
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            _genericGraphEditorView.AddEdge(edge);
        }
    }
}