using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeoTetra.GTLogicGraph.Extensions
{
    public static class VisualElementExtensions
    {
        public static void LoadAndAddStyleSheet(this VisualElement visualElement, string sheetPath)
        {
            StyleSheet styleSheet = Resources.Load(sheetPath, typeof(StyleSheet)) as StyleSheet;
            if ((UnityEngine.Object) styleSheet == (UnityEngine.Object) null)
                Debug.LogWarning((object) string.Format("Style sheet not found for path \"{0}\"", (object) sheetPath));
            else
                visualElement.styleSheets.Add(styleSheet);
        }
    }
}