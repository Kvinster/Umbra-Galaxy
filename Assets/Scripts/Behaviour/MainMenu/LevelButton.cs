using UnityEngine;
using UnityEngine.UI;

using STP.Config;
using STP.Core;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;
using STP.Events;
using STP.Utils.Events;
using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class LevelButton : GameComponent {
		[NotNull] public Button        Button;
		[NotNull] public TMP_Text      LevelText;

		LevelController _levelController;
		LevelNode       _node;

		void OnDestroy() {
			EventManager.Unsubscribe<AllLevelButtonsAreAvailable>(ForceOpenLevel);
			Button.onClick.RemoveAllListeners();
		}

		public void Init(LevelController levelController, LevelNode node) {
			_levelController    = levelController;
			_node               = node;
			Button.interactable = _levelController.IsLevelAvailableToRun(node);
			LevelText.text      = node.LevelName;
			if ( _levelController.IsLevelCompleted(node) ) {
				var colors = Button.colors;
				colors.disabledColor = new Color(0.77f, 0.70f, 0f);
				Button.colors = colors;
			}
			Button.onClick.AddListener(LoadLevel);
			EventManager.Subscribe<AllLevelButtonsAreAvailable>(ForceOpenLevel);
		}

		void ForceOpenLevel(AllLevelButtonsAreAvailable ev) {
			Button.interactable = true;
		}
		
		void LoadLevel() {
			_levelController.StartLevel(_node);
			var clm = CoreLoadingManager.Create();
			if ( clm != null ) {
				UniTask.Void(clm.LoadCore);
			}
		}
	}
}
