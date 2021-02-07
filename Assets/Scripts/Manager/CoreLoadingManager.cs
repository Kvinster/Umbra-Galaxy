using UnityEngine;
using UnityEngine.SceneManagement;

using STP.Behaviour.Starter;

using Cysharp.Threading.Tasks;

namespace STP.Manager {
	public sealed class CoreLoadingManager {
		const string LoadingSceneName = "CoreLoading";
		const string CoreSceneName    = "TestRoom";

		static CoreLoadingManager _instance;

		bool IsLoading { get; set; }

		public async UniTaskVoid LoadCore() {
			await UniTask.SwitchToMainThread();

			IsLoading = true;

			await LoadSceneAsync(LoadingSceneName, LoadSceneMode.Single);
			await LoadSceneAsync(CoreSceneName, LoadSceneMode.Additive);
			await InitLevel();
			await UnloadSceneAsync(LoadingSceneName);

			IsLoading = false;

			UnpauseLevel();
			Release();
		}


		async UniTask LoadSceneAsync(string sceneName, LoadSceneMode mode) {
			await SceneManager.LoadSceneAsync(sceneName, mode);
		}

		async UniTask UnloadSceneAsync(string sceneName) {
			await SceneManager.UnloadSceneAsync(sceneName);
		}

		 async UniTask InitLevel() {
			 var starter = Object.FindObjectOfType<CoreStarter>();
			 if ( starter ) {
				 await starter.InitLevel();
				 starter.PauseManager.Pause(this);
			 } else {
				 Debug.LogErrorFormat("Can't find {0} instance", nameof(CoreStarter));
			 }
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
