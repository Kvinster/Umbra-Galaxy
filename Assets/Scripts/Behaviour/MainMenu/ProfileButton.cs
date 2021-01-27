using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

using System;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.MainMenu {
	public sealed class ProfileButton : GameComponent {
		[NotNull] public Button     MainButton;
		[NotNull] public TMP_Text   Text;
		[NotNull] public GameObject RemoveProfileButtonRoot;
		[NotNull] public Button     RemoveProfileButton;

		public void Init(string text, Action onMainClick, Action onRemoveClick, bool isExists) {
			Assert.IsNotNull(onMainClick);
			Assert.IsTrue(!isExists || (onRemoveClick != null));

			Text.text = text;
			MainButton.onClick.AddListener(onMainClick.Invoke);
			RemoveProfileButtonRoot.SetActive(isExists);
			if ( isExists ) {
				RemoveProfileButton.onClick.AddListener(onRemoveClick.Invoke);
			}
		}

		public void Deinit() {
			MainButton.onClick.RemoveAllListeners();
			RemoveProfileButton.onClick.RemoveAllListeners();
		}
	}
}
