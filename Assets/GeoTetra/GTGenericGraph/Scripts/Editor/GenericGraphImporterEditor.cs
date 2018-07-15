using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
	[CustomEditor(typeof(GenericGraphImporter))]
	public class GenericGraphImporterEditor : ScriptedImporterEditor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Generic Graph Editor"))
			{
				AssetImporter importer = target as AssetImporter;
				Debug.Assert(importer != null, "importer != null");
				ShowGraphEditWindow(importer.assetPath);
			}
		}

		internal static bool ShowGraphEditWindow(string path)
		{
			var guid = AssetDatabase.AssetPathToGUID(path);
			var extension = Path.GetExtension(path);
			if (extension != ".GenericGraph" && extension != ".GenericSubGraph")
				return false;

			var foundWindow = false;
			foreach (var w in Resources.FindObjectsOfTypeAll<GenericGraphEditWindow>())
			{
				if (w.selectedGuid == guid)
				{
					foundWindow = true;
					w.Focus();
				}
			}

			if (!foundWindow)
			{
				GenericGraphEditWindow.CreateWindow(guid);
			}

			return true;
		}

		[OnOpenAsset(0)]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			var path = AssetDatabase.GetAssetPath(instanceID);
			return ShowGraphEditWindow(path);
		}
	}
}