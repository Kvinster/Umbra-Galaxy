using UnityEngine;

using System.Collections;

using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Starter {
	public sealed class LoadingStarter : BaseStarter<LoadingStarter> {
		public float TextChangeInterval = 0.25f;
		[NotNull]
		public TMP_Text Text;

		readonly string[] _texts = { "Loading", "Loading.", "Loading..", "Loading..." };

		Coroutine _textAnim;

		void OnDisable() {
			StopCoroutine(_textAnim);
		}

		void Start() {
			_textAnim = StartCoroutine(TextAnimCoro());
		}

		IEnumerator TextAnimCoro() {
			var textIndex = 0;
			while ( true ) {
				Text.text = _texts[textIndex];
				textIndex = (textIndex + 1) % _texts.Length;
				yield return new WaitForSeconds(TextChangeInterval);
			}
		}
	}
}
