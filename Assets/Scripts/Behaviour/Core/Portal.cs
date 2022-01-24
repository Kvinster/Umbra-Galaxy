using UnityEngine;
using UnityEngine.VFX;

using System;

using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;
using STP.Manager;

namespace STP.Behaviour.Core {
	public class Portal : GameComponent {
		protected const string PortalSizeId = "PortalSize";
		protected const string IgnoreShockwaveId = "IgnoreShockwave";

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

		[Header("Optional dependencies")]
		public CircleCollider2D PortalCollider;

		public BaseSimpleSoundPlayer TargetAppearSoundPlayer;

		Tween _targetAppearAnim;

		PauseManager _pauseManager;

		protected virtual void OnDisable() {
			_targetAppearAnim?.Kill();
		}

		public virtual void Init(CoreStarter coreStarter) {
			_pauseManager = coreStarter.PauseManager;
			gameObject.SetActive(false);
			VisualEffect.Stop();
			_pauseManager.OnIsPausedChanged += OnIsPausedChanged;
			OnIsPausedChanged(_pauseManager.IsPaused);
		}

		public void PrepareTargetAppearAnim(Transform targetTransform, Transform appearPosition) {
			gameObject.SetActive(true);
			var pos = appearPosition.position;
			transform.position         = pos;
			targetTransform.position   = pos;
			targetTransform.localScale = Vector3.zero;
			PreparePortalCollider();
		}

		public void PlayTargetAppearAnim(Transform targetTransform, Transform appearPosition, Action onComplete = null, float startDelay = 0f) {
			var appearTime   = VisualEffect.GetFloat(AppearTimeId);
			var mainLifetime = VisualEffect.GetFloat(AppearMainLifetimeId);
			PrepareTargetAppearAnim(targetTransform, appearPosition);
			VisualEffect.SetFloat(AppearParticlesKillboxSizeId, 0f);
			VisualEffect.SetFloat(PortalSizeId, TargetAppearPortalSize);
			_targetAppearAnim = DOTween.Sequence()
				.AppendInterval(startDelay)
				.AppendCallback(() => {
					if ( TargetAppearSoundPlayer ) {
						TargetAppearSoundPlayer.Play();
					}
					VisualEffect.SendEvent(AppearEventName);
				})
				.Join(DOTween.To(() => VisualEffect.GetFloat(AppearParticlesKillboxSizeId),
					x => VisualEffect.SetFloat(AppearParticlesKillboxSizeId, x), 1f, appearTime).SetEase(Ease.Linear))
				.Join(CreatePortalColliderResizeTween(appearTime))
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
				})
				.SetUpdate(UpdateType.Manual);
		}

		void OnIsPausedChanged(bool isPaused) {
			VisualEffect.pause = isPaused;
		}

		void PreparePortalCollider() {
			if ( !PortalCollider ) {
				return;
			}
			PortalCollider.enabled = false;
		}

		Tween CreatePortalColliderResizeTween(float appearTime) {
			if ( !PortalCollider ) {
				return null;
			}
			var normalPortalColliderRadius = PortalCollider.radius;
			PortalCollider.radius       = 0f;
			PortalCollider.enabled      = false;
			return DOTween.To(() => PortalCollider.radius,
				x => {
					PortalCollider.radius = x;
					if ( !PortalCollider.enabled ) {
						PortalCollider.enabled = true;
					}
				}, normalPortalColliderRadius, appearTime).SetEase(Ease.Linear);
		}
	}
}