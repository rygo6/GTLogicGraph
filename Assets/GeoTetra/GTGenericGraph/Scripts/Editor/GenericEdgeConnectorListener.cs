using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using Edge = UnityEditor.Experimental.UIElements.GraphView.Edge;
using UnityEditor.ShaderGraph.Drawing;

namespace GeoTetra.GTGenericGraph
{
	public class GenericEdgeConnectorListener : IEdgeConnectorListener
	{
		private readonly GenericGraph _graph;
		private readonly GenericSearchWindowProvider _searchWindowProvider;

		public GenericEdgeConnectorListener(GenericGraph graph, GenericSearchWindowProvider searchWindowProvider)
		{
			_graph = graph;
			_searchWindowProvider = searchWindowProvider;
		}

		public void OnDropOutsidePort(Edge edge, Vector2 position)
		{
			var draggedPort = (edge.output != null ? edge.output.edgeConnector.edgeDragHelper.draggedPort : null) ?? (edge.input != null ? edge.input.edgeConnector.edgeDragHelper.draggedPort : null);
			_searchWindowProvider.connectedPort = (GenericPort) draggedPort;
			SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), _searchWindowProvider);
		}

		public void OnDrop(GraphView graphView, Edge edge)
		{
			var leftSlot = edge.output.GetSlot();
			var rightSlot = edge.input.GetSlot();
			if (leftSlot != null && rightSlot != null)
			{
				_graph.RegisterCompleteObjectUndo("Connect Edge");
//				m_Graph.Connect(leftSlot.slotReference, rightSlot.slotReference);
			}
		}
	}
}
