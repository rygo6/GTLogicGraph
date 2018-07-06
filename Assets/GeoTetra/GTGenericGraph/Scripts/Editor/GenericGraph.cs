using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphing;
using UnityEditor.ShaderGraph;

namespace GeoTetra.GTGenericGraph
{
	[Serializable]
	public class GenericGraph : AbstractGenericGraph
	{
		public IMasterNode masterNode
		{
			get { return GetNodes<INode>().OfType<IMasterNode>().FirstOrDefault(); }
		}

		public string GetShader(string name, GenerationMode mode, out List<PropertyCollector.TextureInfo> configuredTextures)
		{
			return masterNode.GetShader(mode, name, out configuredTextures);
		}

		public void LoadedFromDisk()
		{
			OnEnable();
			ValidateGraph();
		}
	}
}
