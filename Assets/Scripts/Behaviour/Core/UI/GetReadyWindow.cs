using UnityEngine;

using STP.Utils.GameComponentAttributes;

using RSG;
using TMPro;

namespace STP.Behaviour.Core.UI {
	public class GetReadyWindow : BaseCoreWindow {
		public float StartPauseTime = 3;

		[NotNull]
		public TMP_Text GetReadySecondsText;

		float _winTime;

		public override IPromise Show() {
			var promise = base.Show();
			_winTime = StartPauseTime;
			return promise;
		}

		protected override void Hide() {
			base.Hide();
			_winTime = float.MaxValue;
		}

		void Update() {
			if ( !IsShown ) {
				return;
			}
			_winTime -= Time.unscaledDeltaTime;
			if (_winTime < 0f) {
				Hide();
			}
			GetReadySecondsText.text = Mathf.CeilToInt(_winTime).ToString();
		}
	}
}