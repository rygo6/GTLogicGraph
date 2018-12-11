using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
    /// <summary>
    /// Implement on a component that should be able to serve as input to a logic graph.
    /// </summary>
    public interface IInputComponent
    {
        event Action<IInputComponent> Changed;
    }
}