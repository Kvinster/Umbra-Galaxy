using UnityEngine.Assertions;
using UnityEngine.UI;

using System;

using STP.Core.State;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class LoadEntry : GameComponent {
		const string DescFormat = "{0}: Level {1}, {2} XP, {3} lives";

		[NotNull] public TMP_Text DescText;
		[NotNull] public Button   LoadButton;

		GameState         _gameState;
		Action<GameState> _startCoreLevel;

		public void Init(GameState gameState, Action<GameState> startCoreLevel) {
			_gameState      = gameState;
			_startCoreLevel = startCoreLevel;
			Assert.IsNotNull(_gameState);
			Assert.IsNotNull(_startCoreLevel);

			DescText.text = string.Format(DescFormat, _gameState.ProfileName, _gameState.LevelState.NextLevelName,
				_gameState.XpState.CurXp, _gameState.PlayerState.CurLives);
			LoadButton.onClick.AddListener(OnLoadClick);
		}

		public void Deinit() {
			_gameState      = null;
			_startCoreLevel = null;

			LoadButton.onClick.RemoveAllListeners();
		}

		void OnLoadClick() {
			_startCoreLevel.Invoke(_gameState);
		}
	}
}
