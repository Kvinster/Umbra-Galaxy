using UnityEngine;

using STP.Behaviour.Starter;
using STP.Controller;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.PowerUps {
	public class RestoreHpPowerUp : BaseCoreComponent {
		const int TempAddLivesValue = 1;

		[NotNull] 
		public TriggerNotifier Notifier;

		PlayerController _playerController;
		
		protected override void InitInternal(CoreStarter starter) {
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
			_playerController.RestoreHp();
			Destroy(gameObject);
		}
	}
}