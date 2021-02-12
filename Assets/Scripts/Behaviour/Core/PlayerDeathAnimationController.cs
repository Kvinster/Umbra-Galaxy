using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using System.Threading;

using STP.Behaviour.Starter;

using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace STP.Behaviour.Core {
	public sealed class PlayerDeathAnimationController : BaseCoreComponent {
		[Range(0f, 10f)]
		public float PlayerDeathAnimDuration;

		ColorAdjustments _colorAdjustments;

		Tween _anim;

		CancellationTokenSource _cancellationTokenSource;

		protected override void OnDisable() {
			base.OnDisable();
			_cancellationTokenSource?.Cancel();
		}

		protected override void InitInternal(CoreStarter starter) {
			var v = starter.MainCamera.GetComponentInChildren<Volume>();
			v.profile.TryGet(out _colorAdjustments);
		}

		public async UniTask PlayPlayerDeathAnim() {
			if ( _anim != null ) {
				Debug.LogError("Anim is already playing");
				return;
			}

			_cancellationTokenSource = new CancellationTokenSource();

			SetProgress(0f);
			var progress = 0f;
			_anim = DOTween.To(() => progress, x => {
				progress = x;
				SetProgress(progress);
			}, 1f, PlayerDeathAnimDuration).SetEase(Ease.OutQuad).SetUpdate(true);
			await _anim.ToUniTask(TweenCancelBehaviour.Kill, _cancellationTokenSource.Token);
			_anim                    = null;
			_cancellationTokenSource = null;
		}

		public void ResetAnim() {
			_cancellationTokenSource?.Cancel();
			_anim                    = null;
			_cancellationTokenSource = null;
			SetProgress(0f);
		}

		void SetProgress(float progress) {
			Time.timeScale                     = 1f - progress;
			_colorAdjustments.saturation.value = -100f * progress;
		}
	}
}
