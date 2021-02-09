using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using STP.Behaviour.Starter;

using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace STP.Behaviour.Core {
	public sealed class PlayerDeathAnimationController : BaseCoreComponent {
		[Range(0f, 10f)]
		public float PlayerDeathAnimDuration;

		ColorAdjustments _colorAdjustments;

		Tween _anim;

		protected override void InitInternal(CoreStarter starter) {
			// _colorAdjustments = VolumeManager.instance.stack.GetComponent<ColorAdjustments>();
			var v = starter.MainCamera.GetComponentInChildren<Volume>();
			v.profile.TryGet(out _colorAdjustments);
		}

		public async UniTask PlayPlayerDeathAnim() {
			if ( _anim != null ) {
				Debug.LogError("Anim is already playing");
				return;
			}
			SetProgress(0f);
			var progress = 0f;
			_anim = DOTween.To(() => progress, x => {
				progress = x;
				SetProgress(progress);
			}, 1f, PlayerDeathAnimDuration).SetEase(Ease.OutQuad).SetUpdate(true);
			await _anim;
			_anim = null;
		}

		public void ResetAnim() {
			if ( _anim != null ) {
				_anim.Kill();
				_anim = null;
			}
			SetProgress(0f);
		}

		void SetProgress(float progress) {
			Time.timeScale                    = 1f - progress;
			_colorAdjustments.saturation.value = -100f * progress;
		}
	}
}
