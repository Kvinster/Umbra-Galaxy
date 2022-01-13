using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

using System;
using System.Threading;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace STP.Behaviour.Core {
	public sealed class PlayerDeathAnimationController : BaseCoreComponent {
		[Header("Parameters")]
		[Header("Not final death anim")]
		public float DiscolorationInAnimDuration  = 3f;
		public float DiscolorationOutAnimDuration = 0.5f;
		public float ExplosionWaitTime            = 3f;
		public float DangerScreenShowDuration     = 0.5f;
		public float DangerScreenHideDuration     = 0.5f;
		[Header("Final death anim")]
		public float FinalDiscolorationAnimDuration = 1f;
		[Header("Dependencies")]
		[Header("Not final death anim")]
		[NotNull] public LevelExplosionZone ExplosionZone;
		[NotNull] public BaseSimpleSoundPlayer DeathSoundPlayer;
		[NotNull] public AudioSource           AudioSource;
		[NotNull] public AudioClip             ShockwaveStartSound;
		[Header("Final death anim")]
		[NotNull] public GameObject FinalDeathVisualEffectRoot;
		[NotNull] public VisualEffect FinalDeathVisualEffect;

		PauseManager       _pauseManager;
		CoreWindowsManager _windowsManager;
		PlayerController   _playerController;

		ColorAdjustments _colorAdjustments;

		CancellationTokenSource _cancellationTokenSource;

		protected override void OnDisable() {
			base.OnDisable();
			_cancellationTokenSource?.Cancel();
		}

		protected override void InitInternal(CoreStarter starter) {
			_pauseManager     = starter.PauseManager;
			_windowsManager   = starter.WindowsManager;
			_playerController = starter.PlayerController;
			var v = starter.MainCamera.GetComponentInChildren<Volume>();
			v.profile.TryGet(out _colorAdjustments);
			ExplosionZone.IgnoreMainEnemies = true;
			ExplosionZone.UseUnscaledTime   = true;
			ExplosionZone.transform.parent = transform.parent;
			ExplosionZone.SetActive(false);

			FinalDeathVisualEffectRoot.SetActive(false);

			AudioSource.ignoreListenerPause = true;
		}

		public async UniTask PlayPlayerDeathAnim() {
			Assert.IsNull(_cancellationTokenSource);
			_cancellationTokenSource = new CancellationTokenSource();

			ExplosionZone.transform.position = transform.position;

			_playerController.IsInvincible = true;

			DeathSoundPlayer.Play();
			await _windowsManager.DangerScreen.Show(DangerScreenShowDuration).ToUniTask(TweenCancelBehaviour.Kill, _cancellationTokenSource.Token);
			await PlayDiscolorationAnim(DiscolorationInAnimDuration, false).ToUniTask(TweenCancelBehaviour.Kill, _cancellationTokenSource.Token);
			await _windowsManager.LivesUi.PlayLoseLiveAnim();
			await PlayDiscolorationAnim(DiscolorationOutAnimDuration, true).ToUniTask(TweenCancelBehaviour.Kill, _cancellationTokenSource.Token);
			ExplosionZone.ResetToStart();
			ExplosionZone.SetActive(true);
			_playerController.RestoreHp();

			AudioSource.PlayOneShot(ShockwaveStartSound);
			await _windowsManager.DangerScreen.Hide(DangerScreenHideDuration).ToUniTask(TweenCancelBehaviour.Kill, _cancellationTokenSource.Token);
			await UniTask.Delay(TimeSpan.FromSeconds(ExplosionWaitTime), true, PlayerLoopTiming.Update, _cancellationTokenSource.Token);
			ExplosionZone.SetActive(false);

			_playerController.IsInvincible = false;

			_cancellationTokenSource = null;
		}

		public async UniTask PlayFinalPlayerDeathAnim() {
			Assert.IsNull(_cancellationTokenSource);
			_cancellationTokenSource = new CancellationTokenSource();

			FinalDeathVisualEffectRoot.SetActive(true);
			FinalDeathVisualEffect.Play();
			await PlayDiscolorationAnim(FinalDiscolorationAnimDuration, false)
				.ToUniTask(TweenCancelBehaviour.Kill, _cancellationTokenSource.Token);

			_windowsManager.ShowDeathWindow();

			_cancellationTokenSource = null;
		}

		public void ResetAnim() {
			_cancellationTokenSource?.Cancel();
			_cancellationTokenSource = null;
			ResetDiscoloration();
			ResetTimeScale();
			FinalDeathVisualEffectRoot.SetActive(false);
		}

		Tween PlayDiscolorationAnim(float duration, bool invert) {
			void SetProgress(float progress) {
				if ( !_pauseManager.IsPaused ) {
					Time.timeScale = 1f - progress;
				}
				_colorAdjustments.saturation.value = -100f * progress;
			}

			SetProgress(invert ? 1f : 0f);
			var progress = 0f;
			return DOTween.To(() => progress, x => {
				progress = x;
				SetProgress(invert ? 1f - progress : progress);
			}, 1f, duration).SetEase(Ease.OutQuad).SetUpdate(true);
		}

		void ResetDiscoloration() {
			_colorAdjustments.saturation.value = 0f;
		}

		void ResetTimeScale() {
			Time.timeScale = 1f;
		}
	}
}