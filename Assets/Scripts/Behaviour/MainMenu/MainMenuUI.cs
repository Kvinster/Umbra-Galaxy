using UnityEngine;
using UnityEngine.UI;

using STP.Behaviour.Core;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public class MainMenuUI : BaseMainMenuComponent {
		[NotNull] public Button StartLevel;

		protected override void InitInternal(MainMenuStarter starter) {
			StartLevel.onClick.AddListener(GoToLevel);
		}

		void OnDestroy() {
			StartLevel.onClick.RemoveAllListeners();
		}

		void GoToLevel() {
			SceneTransitionController.Instance.Transition("TestRoom", Vector3.zero,
				() => {
					var player = FindObjectOfType<Player>();
					return player ? player.transform.position : Vector3.zero;
				}).Then(() => { Time.timeScale = 1f; });
		}
	}
}