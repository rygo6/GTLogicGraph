using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using Edge = UnityEditor.Experimental.UIElements.GraphView.Edge;
using UnityEditor.ShaderGraph.Drawing;

namespace GeoTetra.GTGenericGraph
{
	class GenericEdgeConnectorListener : IEdgeConnectorListener
	{
		readonly AbstractGenericGraph m_Graph;
		readonly GenericSearchWindowProvider m_SearchWindowProvider;

		public GenericEdgeConnectorListener(AbstractGenericGraph graph, GenericSearchWindowProvider searchWindowProvider)
		{
			m_Graph = graph;
			m_SearchWindowProvider = searchWindowProvider;
		}

		public void OnDropOutsidePort(Edge edge, Vector2 position)
		{
			var draggedPort = (edge.output != null ? edge.output.edgeConnector.edgeDragHelper.draggedPort : null) ?? (edge.input != null ? edge.input.edgeConnector.edgeDragHelper.draggedPort : null);
			m_SearchWindowProvider.connectedPort = (GenericPort) draggedPort;
			SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), m_SearchWindowProvider);
		}

		public void OnDrop(GraphView graphView, Edge edge)
		{
			var leftSlot = edge.output.GetSlot();
			var rightSlot = edge.input.GetSlot();
			if (leftSlot != null && rightSlot != null)
			{
				m_Graph.owner.RegisterCompleteObjectUndo("Connect Edge");
				m_Graph.Connect(leftSlot.slotReference, rightSlot.slotReference);
			}
		}
	}
}
