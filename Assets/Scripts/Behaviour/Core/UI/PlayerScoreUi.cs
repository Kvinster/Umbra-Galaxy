using UnityEngine;

using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;
using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class PlayerScoreUi : GameComponent {
		const string ScoreTextTemplate = "Score: {0}";

		[Header("Parameters")]
		public float FadeDuration = 0.2f;
		public float ScoreChangeDuration = 0.5f;
		public float ShowDuration        = 2f;
		[Header("Dependencies")]
		[NotNull] public CanvasGroup CanvasGroup;
		[NotNull] public TMP_Text    ScoreText;

		ScoreController _scoreController;

		Sequence _anim;

		int  _curScore;

		public void Init(ScoreController scoreController) {
			_scoreController                      =  scoreController;
			_scoreController.Score.OnValueChanged += OnScoreChanged;

			_curScore = _scoreController.Score;

			CanvasGroup.alpha = 0f;

			UpdateScoreText();
		}

		public void Deinit() {
			_anim?.Kill();
			if ( _scoreController != null ) {
				_scoreController.Score.OnValueChanged -= OnScoreChanged;
			}
		}

		void OnScoreChanged(int score) {
			_anim?.Kill();
			_anim = DOTween.Sequence()
				.Append(PlayFadeAnim(true, false))
				.Append(PlayTextChangeAnim(score))
				.AppendInterval(ShowDuration - ScoreChangeDuration)
				.Append(PlayFadeAnim(false, true));
		}

		Tween PlayFadeAnim(bool appear, bool delayedStart) {
			var endValue = appear ? 1f : 0f;
			var curAlpha = delayedStart ? (appear ? 0f : 1f) : CanvasGroup.alpha;
			var duration = appear ? ((1f - curAlpha) * FadeDuration) : (curAlpha * FadeDuration);
			return DOTween.Sequence().Append(CanvasGroup.DOFade(endValue, duration));
		}

		Tween PlayTextChangeAnim(int score) {
			return DOTween.Sequence()
				.Append(DOTween.To(() => _curScore, x => {
						_curScore = x;
						UpdateScoreText();
					}, score, ScoreChangeDuration)
					.SetEase(Ease.Linear));
		}

		void UpdateScoreText() {
			ScoreText.text = string.Format(ScoreTextTemplate, _curScore);
		}
	}
}
