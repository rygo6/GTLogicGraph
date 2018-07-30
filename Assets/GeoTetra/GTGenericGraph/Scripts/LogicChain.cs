using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
	public class LogicChain
	{
		public event Action<float> output;
		
		public readonly Type outputType;
		public Type inputType;
		
		public LogicChain(Type outputType)
		{
			
		}

		public void OnOutput(float value)
		{
			if (output != null) output(value);
		}
	}

//	public class LogicDescription
//	{
//		public string Guid;
//		public List<string> 
//	}
//	
//	public struct Slo
}