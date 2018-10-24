# GTLogicGraph

![Previe](Preview.PNG)

The intent of this project is to provide a generic graph view based on the new GraphView and VisualElements system found in Unity 2018, the same systems which the new ShaderGraph are based off of.

Completed or Started:
- Can drag graph around.
- Can zoom in/out of graph.
- Can drag nodes around.
- Can drag a box to multi select nodes and drag multiple nodes.
- Can drag line off of node.
- Can chain together nodes.
- Can open search menu.
- Serializing and save of graphs.
- Graphs can be loaded into scene via LogicGraphInstance component.

Still to do:
- Implement more common base types and nodes.
- Copy and paste nodes.

Bugs
- After you clicks 'save asset' in graph, you must hit ctrl+S for the reference in a scene to update. Why?

## Installation

This repo is intended to cloned as a submodule into a unity project.
```
git submodule add https://github.com/rygo6/GTLogicGraph.git Assets/GeoTetra/GTLogicGraph
```
