using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;

namespace GeoTetra.GTGenericGraph
{
	[CustomEditor(typeof(GenericGraphImporter))]
	public class GenericGraphImporterEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Generic Graph Editor"))
			{
				AssetImporter importer = target as AssetImporter;
				ShowGraphEditWindow(importer.assetPath);
			}
		}

		private static bool ShowGraphEditWindow(string path)
		{
			var guid = AssetDatabase.AssetPathToGUID(path);
			var extension = Path.GetExtension(path);
			if (extension != ".GenericGraph" && extension != ".GenericSubGraph")
				return false;

			var foundWindow = false;
			foreach (var w in Resources.FindObjectsOfTypeAll<GenericGraphEditorWindow>())
			{
				if (w.SelectedGuid == guid)
				{
					foundWindow = true;
					w.Focus();
				}
			}

			if (!foundWindow)
			{
				var window = CreateInstance<GenericGraphEditorWindow>();
				window.Show();
				window.Initialize(guid);
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