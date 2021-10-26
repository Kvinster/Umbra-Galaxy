using UnityEngine;
using UnityEngine.SceneManagement;

namespace STP.Service {
	public static class SceneService {
		const string LevelSceneNamePrefix = "Level_";
		const string CommonLevelSceneName = "Level_Common";

		public static void LoadMainMenu() {
			SceneManager.LoadScene("MainMenu");
		}

		public static void ReloadScene() {
			if ( IsLevelScene() ) {
				LoadLevel(GetLevelIndexFromSceneName());
			} else {
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}

		public static void LoadLevel(int levelIndex) {
			SceneManager.LoadScene(LevelSceneNamePrefix + levelIndex, LoadSceneMode.Single);
			SceneManager.LoadScene(CommonLevelSceneName, LoadSceneMode.Additive);
		}

		public static int GetLevelIndexFromSceneName() {
			var sceneName = SceneManager.GetActiveScene().name;
			if ( !IsLevelScene() ) {
				Debug.LogErrorFormat("SceneService.GetLevelIndexFromSceneName: invalid scene name '{0}'", sceneName);
				return -1;
			}
			return int.Parse(sceneName.Substring(LevelSceneNamePrefix.Length));
		}

		public static void CheatLoadLevelCommonScene() {
			SceneManager.LoadScene(CommonLevelSceneName, LoadSceneMode.Additive);
		}

		static bool IsLevelScene() {
			var sceneName = SceneManager.GetActiveScene().name;
			return sceneName.StartsWith(LevelSceneNamePrefix);
		}
	}
}
