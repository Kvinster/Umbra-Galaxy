using UnityEngine;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Controller;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.PowerUps {
	public sealed class ShieldPowerUp : BaseCoreComponent {
		public const float TmpShieldDuration = 10f;

		[NotNull] public TriggerNotifier Notifier;

		PlayerManager    _playerManager;
		PlayerController _playerController;

		protected override void InitInternal(CoreStarter starter) {
			_playerManager          =  starter.PlayerManager;
			_playerController       =  PlayerController.Instance;
			Notifier.OnTriggerEnter += OnRangeEnter;
		}

		void OnDestroy() {
			Notifier.OnTriggerEnter -= OnRangeEnter;
		}

		void OnRangeEnter(GameObject go) {
			var playerComp = go.GetComponent<Player>();
			if ( !playerComp ) {
				return;
			}
			_playerController.IsInvincible = true;
			_playerManager.AddTimeToPowerUp(PowerUpNames.Shield, TmpShieldDuration);
			Destroy(gameObject);
		}
	}
}
