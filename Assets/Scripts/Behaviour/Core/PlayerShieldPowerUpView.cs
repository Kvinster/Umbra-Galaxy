using UnityEngine.VFX;

using STP.Behaviour.Core.PowerUps;
using STP.Behaviour.Starter;
using STP.Common;
using STP.Controller;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class PlayerShieldPowerUpView : BaseCoreComponent {
		const float ActiveLifetime       = 5f;
		const float InactiveLifetime     = 0f;
		const float ActiveAttractSpeed   = 100f;
		const float InactiveAttractSpeed = 0f;
		const float ActiveEscapeSpeed    = 0f;
		const float InactiveEscapeSpeed  = 100f;
		const float StartIntensity       = 3f;
		const float FinishIntensity      = 0f;

		[NotNull] public VisualEffect VisualEffect;

		PlayerManager    _playerManager;
		PlayerController _playerController;

		bool _isActive;

		void OnDestroy() {
			if ( _playerController != null ) {
				_playerController.OnIsInvincibleChanged -= OnIsInvincibleChanged;
			}
		}

		void Update() {
			if ( !_isActive ) {
				return;
			}
			var coef = 1f - _playerManager.GetPowerUpTime(PowerUpNames.Shield) / ShieldPowerUp.TmpShieldDuration;
			VisualEffect.SetFloat("Intensity", MathUtils.LerpFloat(StartIntensity, FinishIntensity, coef * coef * coef));
		}

		protected override void InitInternal(CoreStarter starter) {
			_playerManager = starter.PlayerManager;

			VisualEffect.Stop();

			_playerController = starter.PlayerController;
			_playerController.OnIsInvincibleChanged += OnIsInvincibleChanged;
		}

		void OnIsInvincibleChanged(bool isInvincible) {
			if ( isInvincible ) {
				VisualEffect.SetFloat("Lifetime", ActiveLifetime);
				VisualEffect.SetFloat("AttractSpeed", ActiveAttractSpeed);
				VisualEffect.SetFloat("EscapeSpeed", ActiveEscapeSpeed);
				VisualEffect.SetFloat("Intensity", StartIntensity);
				VisualEffect.Play();
				_isActive = true;
			} else {
				VisualEffect.SetFloat("Lifetime", InactiveLifetime);
				VisualEffect.SetFloat("AttractSpeed", InactiveAttractSpeed);
				VisualEffect.SetFloat("EscapeSpeed", InactiveEscapeSpeed);
				VisualEffect.Stop();
				_isActive = false;
			}
		}
	}
}
