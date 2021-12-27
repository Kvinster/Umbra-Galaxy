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
			Cursor.SetCursor(SceneService.IsSceneLoaded("MainMenu") ? MainMenuCursor : CoreCursor, Vector2.zero, CursorMode.Auto);
		}

		[RuntimeInitializeOnLoadMethod]
		static void Initialize() {
			var go = Instantiate(Resources.Load<CursorController>(PrefabPath));
			DontDestroyOnLoad(go);
		}
	}
}
