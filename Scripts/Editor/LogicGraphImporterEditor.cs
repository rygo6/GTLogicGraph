using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

namespace GeoTetra.GTLogicGraph
{
	[CustomEditor(typeof(LogicGraphImporter))]
	public class LogicGraphImporterEditor : ScriptedImporterEditor
	{
		public override void OnInspectorGUI()
		{
//			if (GUILayout.Button("Open Generic Graph Editor"))
//			{
//				AssetImporter importer = target as AssetImporter;
//				ShowGraphEditWindow(importer.assetPath);
//			}

			DrawDefaultInspector();
		}

		private static bool ShowGraphEditWindow(string path)
		{
			var guid = AssetDatabase.AssetPathToGUID(path);
			var extension = Path.GetExtension(path);
			if (extension != ".LogicGraph" && extension != ".LogicGraph")
				return false;

			var foundWindow = false;
			foreach (var w in Resources.FindObjectsOfTypeAll<LogicGraphEditorWindow>())
			{
				if (w.SelectedGuid == guid)
				{
					foundWindow = true;
					w.Focus();
				}
			}

			if (!foundWindow)
			{
				var window = CreateInstance<LogicGraphEditorWindow>();
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