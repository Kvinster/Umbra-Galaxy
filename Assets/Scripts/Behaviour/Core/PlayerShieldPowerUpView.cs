using UnityEngine;
using UnityEngine.VFX;

using STP.Behaviour.Core.PowerUps;
using STP.Behaviour.Starter;
using STP.Common;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class PlayerShieldPowerUpView : BaseCoreComponent {
		readonly struct VisualEffectParams {
			public readonly float Lifetime;
			public readonly float AttractSpeed;
			public readonly float EscapeSpeed;

			public VisualEffectParams(float lifetime, float attractSpeed, float escapeSpeed) {
				Lifetime     = lifetime;
				AttractSpeed = attractSpeed;
				EscapeSpeed  = escapeSpeed;
			}
		}

		const float StartIntensity  = 3f;
		const float FinishIntensity = 0f;

		static readonly VisualEffectParams ActiveVisualEffectParams =
			new VisualEffectParams(lifetime: 5f, attractSpeed: 100f, escapeSpeed: 0f);
		static readonly VisualEffectParams InactiveVisualEffectParams =
			new VisualEffectParams(lifetime: 0f, attractSpeed: 0f, escapeSpeed: 100f);

		[NotNull] public VisualEffect VisualEffect;
		[NotNull] public GameObject   ShieldRoot;

		PlayerManager _playerManager;

		bool _isActive;

		void OnDestroy() {
			if ( _playerManager != null ) {
				_playerManager.OnPowerUpStarted  -= OnPowerUpStarted;
				_playerManager.OnPowerUpFinished -= OnPowerUpFinished;
			}
		}

		void Update() {
			if ( !_isActive ) {
				return;
			}
			var coef = 1f - _playerManager.GetPowerUpCurTime(PowerUpType.Shield) /
				_playerManager.GetPowerUpTotalTime(PowerUpType.Shield);
			VisualEffect.SetFloat("Intensity", MathUtils.LerpFloat(StartIntensity, FinishIntensity, coef * coef * coef));
		}

		protected override void InitInternal(CoreStarter starter) {
			_playerManager = starter.PlayerManager;

			VisualEffect.SendEvent("Deactivate");
			ShieldRoot.SetActive(false);

			_playerManager.OnPowerUpStarted  += OnPowerUpStarted;
			_playerManager.OnPowerUpFinished += OnPowerUpFinished;
		}

		void OnPowerUpStarted(PowerUpType type) {
			if ( type != PowerUpType.Shield ) {
				return;
			}
			SetVisualEffectParams(ActiveVisualEffectParams);
			VisualEffect.SetFloat("Intensity", StartIntensity);
			VisualEffect.SendEvent("Activate");
			ShieldRoot.SetActive(true);
			_isActive = true;
		}

		void OnPowerUpFinished(PowerUpType type) {
			if ( type != PowerUpType.Shield ) {
				return;
			}
			SetVisualEffectParams(InactiveVisualEffectParams);
			VisualEffect.SendEvent("Deactivate");
			ShieldRoot.SetActive(false);
			_isActive = false;
		}

		void SetVisualEffectParams(VisualEffectParams visualEffectParams) {
			VisualEffect.SetFloat("Lifetime", visualEffectParams.Lifetime);
			VisualEffect.SetFloat("AttractSpeed", visualEffectParams.AttractSpeed);
			VisualEffect.SetFloat("EscapeSpeed", visualEffectParams.EscapeSpeed);
		}
	}
}
