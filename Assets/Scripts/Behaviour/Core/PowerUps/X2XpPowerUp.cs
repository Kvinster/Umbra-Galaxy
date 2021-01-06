using UnityEngine;

using STP.Behaviour.Starter;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.PowerUps {
	public class X2XpPowerUp : BaseCoreComponent {
		const int TempAddPowerUpTimeSec = 10;

		[NotNull] 
		public TriggerNotifier Notifier;

		PlayerManager _playerManager;
		
		protected override void InitInternal(CoreStarter starter) {
			_playerManager          =  starter.PlayerManager;
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
			_playerManager.AddTimeToPowerUp(PowerUpNames.X2Xp, TempAddPowerUpTimeSec);
			Destroy(gameObject);
		}
	}
}