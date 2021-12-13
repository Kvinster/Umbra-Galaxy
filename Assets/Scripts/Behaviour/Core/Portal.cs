using UnityEngine;
using UnityEngine.VFX;

using System;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

namespace STP.Behaviour.Core {
	public class Portal : GameComponent {
		protected const string PortalSizeId = "PortalSize";

		const string AppearEventName              = "OnAppear";
		const string AppearTimeId                 = "AppearBorder_AppearTime";
		const string AppearMainLifetimeId         = "AppearBorder_MainLifetime";
		const string AppearDisappearTimeId        = "AppearBorder_DisappearTime";
		const string AppearParticlesKillboxSizeId = "AppearBorder_ParticlesKillboxSize";

		[Header("Base Parameters")]
		public float TargetScaleTime = 1f;
		public float TargetAppearPortalSize = 1f;
		[Header("Base Dependencies")]
		[NotNull] public VisualEffect VisualEffect;

		Tween _targetAppearAnim;

		protected virtual void OnDisable() {
			_targetAppearAnim?.Kill();
		}

		public virtual void Init(CoreStarter coreStarter) {
			gameObject.SetActive(false);
			VisualEffect.Stop();
		}

		public void PlayTargetAppearAnim(Transform targetTransform, Transform appearPosition, Action onComplete = null,
			float startDelay = 0f) {
			gameObject.SetActive(true);
			var pos          = appearPosition.position;
			var appearTime   = VisualEffect.GetFloat(AppearTimeId);
			var mainLifetime = VisualEffect.GetFloat(AppearMainLifetimeId);
			VisualEffect.SetFloat(AppearParticlesKillboxSizeId, 0f);
			VisualEffect.SetFloat(PortalSizeId, TargetAppearPortalSize);
			transform.position         = pos;
			targetTransform.position   = pos;
			targetTransform.localScale = Vector3.zero;
			_targetAppearAnim = DOTween.Sequence()
				.AppendInterval(startDelay)
				.AppendCallback(() => VisualEffect.SendEvent(AppearEventName))
				.Join(DOTween.To(() => VisualEffect.GetFloat(AppearParticlesKillboxSizeId),
					x => VisualEffect.SetFloat(AppearParticlesKillboxSizeId, x), 1f, appearTime).SetEase(Ease.Linear))
				.Insert(startDelay + appearTime, targetTransform.DOScale(Vector3.one, TargetScaleTime))
				.Insert(startDelay + appearTime + mainLifetime,
					DOTween.To(() => VisualEffect.GetFloat(AppearParticlesKillboxSizeId),
						x => VisualEffect.SetFloat(AppearParticlesKillboxSizeId, x), 0f,
						VisualEffect.GetFloat(AppearDisappearTimeId)).SetEase(Ease.Linear))
				.OnComplete(() => {
					_targetAppearAnim = null;
					VisualEffect.Stop();
					gameObject.SetActive(false);
					onComplete?.Invoke();
				});
		}
	}
}
