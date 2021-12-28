using UnityEngine;
using UnityEngine.SceneManagement;

using STP.Service;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Utils {
	public sealed class CursorController : MonoBehaviour {
		const string PrefabPath = "CursorController";

		[NotNull] public Texture2D CoreCursor;
		[NotNull] public Texture2D MainMenuCursor;

		void Start() {
			UpdateCursor();
			SceneManager.activeSceneChanged += OnActiveSceneChanged;
		}

		void OnActiveSceneChanged(Scene arg0, Scene arg1) {
			UpdateCursor();
		}

		void UpdateCursor() {
			var isMainMenu = SceneService.IsSceneLoaded("MainMenu");
			Cursor.SetCursor(isMainMenu ? MainMenuCursor : CoreCursor, isMainMenu ? new Vector2(0, 0) : new Vector2(32, 32), CursorMode.Auto);
		}

		[RuntimeInitializeOnLoadMethod]
		static void Initialize() {
			var go = Instantiate(Resources.Load<CursorController>(PrefabPath));
			DontDestroyOnLoad(go);
		}
	}
}
