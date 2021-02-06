using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;

using STP.Behaviour.Starter;
using STP.Utils;

using RSG;

namespace STP.Manager {
	public sealed class CoreLoadingManager {
		const string LoadingSceneName = "CoreLoading";
		const string CoreSceneName    = "TestRoom";

		static CoreLoadingManager _instance;

		bool IsLoading { get; set; }

		public void LoadCore() {
			IsLoading = true;
			LoadSceneAsync(LoadingSceneName, LoadSceneMode.Single)
				.Then(() => LoadSceneAsync(CoreSceneName, LoadSceneMode.Additive))
				.Then(InitLevel)
				.Then(() => UnloadSceneAsync(LoadingSceneName))
				.Then(() => {
					IsLoading = false;

					UnpauseLevel();
					Release();
				});
		}


		IPromise LoadSceneAsync(string sceneName, LoadSceneMode mode) {
			var promise = new Promise();
			UnityContext.Instance.StartCoroutine(LoadSceneAsyncCoro(sceneName, mode, promise));
			return promise;
		}

		IEnumerator LoadSceneAsyncCoro(string sceneName, LoadSceneMode mode, Promise promise) {
			var op = SceneManager.LoadSceneAsync(sceneName, mode);
			while ( !op.isDone ) {
				yield return null;
			}
			promise.Resolve();
		}

		IPromise UnloadSceneAsync(string sceneName) {
			var promise = new Promise();
			UnityContext.Instance.StartCoroutine(UnloadSceneAsyncCoro(sceneName, promise));
			return promise;
		}

		IEnumerator UnloadSceneAsyncCoro(string sceneName, Promise promise) {
			var op = SceneManager.UnloadSceneAsync(sceneName);
			while ( !op.isDone ) {
				yield return null;
			}
			promise.Resolve();
		}

		IPromise InitLevel() {
			var promise = new Promise();
			UnityContext.Instance.StartCoroutine(InitLevelCoro(promise));
			return promise;
		}

		IEnumerator InitLevelCoro(Promise promise) {
			var starter = Object.FindObjectOfType<CoreStarter>();
			if ( starter ) {
				yield return starter.InitLevel();
				starter.PauseManager.Pause(this);
			} else {
				Debug.LogErrorFormat("Can't find {0} instance", nameof(CoreStarter));
			}
			promise.Resolve();
		}

		void UnpauseLevel() {
			var starter = Object.FindObjectOfType<CoreStarter>();
			if ( starter ) {
				starter.PauseManager.Unpause(this);
			} else {
				Debug.LogErrorFormat("Can't find {0} instance", nameof(CoreStarter));
			}
		}

		public static CoreLoadingManager Create() {
			if ( _instance != null ) {
				Debug.LogErrorFormat("Another instance of {0} already exists", nameof(CoreLoadingManager));
			}
			_instance = new CoreLoadingManager();
			return _instance;
		}

		static void Release() {
			if ( _instance == null ) {
				Debug.LogErrorFormat("No instance of {0} exists", nameof(CoreLoadingManager));
				return;
			}
			if ( _instance.IsLoading ) {
				Debug.LogErrorFormat("Can't release {0} instance when loading is in progress", nameof(CoreLoadingManager));
				return;
			}
			_instance = null;
		}
	}
}
