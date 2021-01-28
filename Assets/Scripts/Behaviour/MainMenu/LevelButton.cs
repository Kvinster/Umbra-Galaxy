using UnityEngine.UI;
using UnityEngine.Assertions;

using System;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class LevelButton : GameComponent {
		[NotNull] public Button   Button;
		[NotNull] public TMP_Text LevelText;

		public void Init(Action loadLevel, int levelIndex, bool isActive) {
			Assert.IsNotNull(loadLevel);
			Assert.IsTrue(levelIndex >= 0);

			Button.interactable = isActive;
			if ( isActive ) {
				Button.onClick.AddListener(loadLevel.Invoke);
			}
			LevelText.text = (levelIndex + 1).ToString();
		}

		public void Deinit() {
			Button.onClick.RemoveAllListeners();
		}
	}
}
