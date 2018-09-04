using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GeoTetra.GTGenericGraph
{
	public interface IInputMethod
	{

	}
	
	public interface IFloatInput : IInputMethod
	{
		void SetValue(float value);
	}
}