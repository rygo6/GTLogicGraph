# GraphViewExample

The intent of this project is to provide a generic graph view based on the new GraphView and VisualElements system found in Unity 2018, the same systems which the new ShaderGraph are based off of.

Currently this repo contains the source code of the GraphView and the source code of the visual element examples, alongside the source for the generic graph view in the GeoTetra/GTGenericGraph directory. This is done to easily cross reference and pull source from the ShaderGraph source, as much as possible the patterns of the ShaderGraph should be followed, and its classes relied upon.

The intent is to create a feature filled basis off of which the new GraphView can be utilized to support any system. 

Completed or Started:
- Can open GenericGraphView from Windows > GenericGraph.
- Can right click to create new node.
- Can drag graph around.
- Can zoom in/out of graph.
- Can drag nodes around.
- Can drag a box to multi select nodes and drag multiple nodes.
- Can adjust default input port.
- Can drag line off of node.
- Can chain together nodes.
- Can open search menu.
- Serializing and save of graphs. Loading graph.
- Graphs can be loaded into scene via GraphLogic component

Still to do:
- Implement more common base types and nodes.
- copy and paste nodes

Bugs
- After you clicks 'save asset' in graph, you must hit ctrl+S for the reference in a scene to update. Why?
