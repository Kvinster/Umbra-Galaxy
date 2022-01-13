using STP.Core;
using STP.Service;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace STP.Behaviour.MainMenu {
	public sealed class CheatPanel : MonoBehaviour {
		public GameObject     Root;
		public TMP_InputField InputField;
		public Button         Button;

		void Start() {
			Button.onClick.AddListener(OnButtonClick);
		}

		void Update() {
			Root.SetActive(CheatsService.ShowCheatPanel);
		}

		void OnButtonClick() {
			if ( !int.TryParse(InputField.text, out var levelIndexRaw) || (levelIndexRaw <= 0) || (levelIndexRaw > 15) ) {
				return;
			}
			var levelIndex = levelIndexRaw - 1;
			GameController.Instance.LevelController.StartLevel(levelIndex);
			SceneService.LoadLevel(GameController.Instance.LevelController.CurLevelConfig.SceneName);
		}
	}
}