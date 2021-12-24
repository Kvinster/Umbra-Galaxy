using UnityEngine;
using UnityEngine.SceneManagement;

namespace STP.Service {
	public static class SceneService {
		const string LevelSceneNamePrefix = "Level_";
		const string CommonLevelSceneName = "Level_Common";

		public static bool IsLevelCommonSceneLoaded => IsSceneLoaded(CommonLevelSceneName);

		public static void LoadMainMenu() {
			SceneManager.LoadScene("MainMenu");
		}


		public static void LoadLevel(string sceneName) {
			SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
			SceneManager.LoadScene(CommonLevelSceneName, LoadSceneMode.Additive);
		}

		public static void CheatLoadLevelCommonScene() {
			SceneManager.LoadScene(CommonLevelSceneName, LoadSceneMode.Additive);
		}

		public static bool IsSceneLoaded(string sceneName) {
			for ( var sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++ ) {
				var scene = SceneManager.GetSceneAt(sceneIndex);
				if ( scene.name == sceneName ) {
					return true;
				}
			}
			return false;
		}

	}
}