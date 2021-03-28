using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;
using TMPro;

namespace STP.Behaviour.EndlessLevel {
	public class WaveTextEffect : BaseEndlessLevelComponent {
		[NotNull] public TMP_Text  WaveText;
		[NotNull] public Transform EndPoint;

		public float FadeOutDuration;
		public float FadeInDuration;
		
		Vector3  _startPoint;
		Sequence _sequence;

		WaveController _waveController;
		
		protected override void InitInternal(EndlessLevelStarter starter) {
			_startPoint                   =  WaveText.transform.position;
			_waveController               =  starter.WaveController;
			_waveController.OnWaveStarted += ShowWaveEffect;
			WaveText.alpha                =  0;
		}

		void OnDestroy() {
			if ( !_waveController ) {
				_waveController.OnWaveStarted -= ShowWaveEffect;
			}
		}
		
		void ShowWaveEffect(int waveIndex) {
			_sequence?.Kill();
			_sequence                   = DOTween.Sequence();
			WaveText.alpha              = 0;
			WaveText.transform.position = _startPoint;
			WaveText.text               = $"Wave {waveIndex}";
			_sequence.Append(WaveText.DOFade(1f, FadeInDuration));
			_sequence.Insert(FadeInDuration, WaveText.DOFade(0f, FadeOutDuration));
			_sequence.Insert(FadeInDuration, WaveText.transform.DOLocalMove(EndPoint.position, FadeOutDuration));
		}
	}
}