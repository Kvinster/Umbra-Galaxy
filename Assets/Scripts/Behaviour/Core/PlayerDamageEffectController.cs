using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using System.Threading;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core {
	public sealed class PlayerDamageEffectController : BaseCoreComponent {
		[NotNull] public DamageVignette DamageVignette;

		ChromaticAberration     _chromaticAberration;
		ColorAdjustments        _colorAdjustments;
		CancellationTokenSource _damageEffectCancellationTokenSource;

		Player _player;

		protected override void OnDisable() {
			base.OnDisable();

			_damageEffectCancellationTokenSource?.Cancel();

			if ( _player ) {
				_player.OnPlayerTakeDamage -= OnPlayerTakeDamage;
				_player.OnPlayerRespawn    -= OnPlayerRespawn;
				_player.OnPlayerDied       -= OnPlayerDied;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_player = starter.Player;

			var volume = starter.MainCamera.GetComponent<Volume>();
			volume.profile.TryGet(out _chromaticAberration);
			volume.profile.TryGet(out _colorAdjustments);
			Assert.IsTrue(_chromaticAberration);
			Assert.IsTrue(_colorAdjustments);

			_player.OnPlayerTakeDamage += OnPlayerTakeDamage;
			_player.OnPlayerRespawn    += OnPlayerRespawn;
			_player.OnPlayerDied       += OnPlayerDied;

			OnPlayerRespawn();
		}

		void OnPlayerTakeDamage() {
			_damageEffectCancellationTokenSource?.Cancel();
			_damageEffectCancellationTokenSource = new CancellationTokenSource();
			UniTask.Void(DamageEffect, _damageEffectCancellationTokenSource.Token);

		}

		void OnPlayerRespawn() {
			_damageEffectCancellationTokenSource?.Cancel();
			SetDamageEffect(0f);
		}

		void OnPlayerDied() {
			_damageEffectCancellationTokenSource?.Cancel();
			SetDamageEffect(1f);
		}

		async UniTaskVoid DamageEffect(CancellationToken cancellationToken) {
			var value    = 1.0f;
			var progress = 0f;
			SetDamageEffect(value);
			while ( value > 0f ) {
				if ( cancellationToken.IsCancellationRequested ) {
					return;
				}
				progress = Mathf.Clamp01(progress + Time.deltaTime);
				value    = 1f - Mathf.Pow(progress, 5f);
				SetDamageEffect(value);
				await UniTask.WaitForEndOfFrame(cancellationToken);
			}
		}

		void SetDamageEffect(float value) {
			_chromaticAberration.intensity.value = value;
			_colorAdjustments.saturation.value   = 100f * value;
			DamageVignette.SetEffectValue(0.6f * value);
		}
	}
}
