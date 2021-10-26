using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

using UnityToolbarExtender;

namespace STP.Editor {
	[InitializeOnLoad]
	public static class ToolbarGui {
		static ToolbarGui() {
			ToolbarExtender.LeftToolbarGUI.Add(OnLeftToolbarGUI);
			ToolbarExtender.RightToolbarGUI.Add(OnRightToolbarGUI);
		}

		static void OnLeftToolbarGUI() {
			if ( GUILayout.Button("Add Generator Scene", GUILayout.ExpandWidth(false)) ) {
				for ( var i = 0; i < EditorSceneManager.loadedSceneCount; ++i ) {
					var scene = SceneManager.GetSceneAt(i);
					if ( scene.name == "Generator" ) {
						return;
					}
				}
				EditorSceneManager.OpenScene("Assets/Scenes/Generator.unity", OpenSceneMode.Additive);
			}
		}

		static void OnRightToolbarGUI() {

		}
	}
}
