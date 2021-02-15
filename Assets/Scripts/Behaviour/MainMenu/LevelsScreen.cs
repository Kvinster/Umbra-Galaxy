using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Config;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.MainMenu {
	public sealed class LevelsScreen : BaseMainMenuComponent {
		[NotNull] public Transform  LevelButtonParent;
		[NotNull] public GameObject LevelButtonPrefab;
		[NotNull] public Button     BackButton;

		MainMenuManager _mainMenuManager;

		readonly List<LevelButton> _activeLevelButtons   = new List<LevelButton>();
		readonly List<LevelButton> _inactiveLevelButtons = new List<LevelButton>();

		protected override void InitInternal(MainMenuStarter starter) {
			_mainMenuManager = starter.MainMenuManager;

			BackButton.onClick.AddListener(OnBackClick);
		}

		public void Show() {
			Assert.IsTrue(ProfileController.IsActiveInstanceExists);

			var ps          = ProfileController.ActiveInstance;
			var levelConfig = Resources.Load<LevelsConfig>("AllLevels");
			for ( var i = 0; i < levelConfig.Levels.Count; ++i ) {
				var levelButton = GetFreeLevelButton();
				var levelIndex  = i;
				levelButton.Init(() => LoadLevel(levelIndex), levelIndex,
					levelIndex <= ps.ProfileState.LevelState.LastLevelIndex);
				_activeLevelButtons.Add(levelButton);
			}

			gameObject.SetActive(true);
		}

		public void Hide() {
			ResetLevelButtons();

			gameObject.SetActive(false);
		}

		void OnBackClick() {
			_mainMenuManager?.ShowMain();
		}

		void LoadLevel(int levelIndex) {
			ProfileController.ActiveInstance.StartLevel(levelIndex);
			var clm = CoreLoadingManager.Create();
			if ( clm != null ) {
				UniTask.Void(clm.LoadCore);
			}
		}

		void ResetLevelButtons() {
			foreach ( var lb in _activeLevelButtons ) {
				lb.Deinit();
				lb.gameObject.SetActive(false);
				_inactiveLevelButtons.Add(lb);
			}
			_activeLevelButtons.Clear();
		}

		LevelButton GetFreeLevelButton() {
			if ( _inactiveLevelButtons.Count > 0 ) {
				var lb = _inactiveLevelButtons[0];
				_inactiveLevelButtons.RemoveAt(0);
				lb.gameObject.SetActive(true);
				return lb;
			} else {
				var lbGo = Instantiate(LevelButtonPrefab, LevelButtonParent);
				lbGo.transform.SetAsLastSibling();
				var lb = lbGo.GetComponent<LevelButton>();
				return lb;
			}
		}
	}
}
