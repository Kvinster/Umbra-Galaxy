using UnityEngine;

using STP.Behaviour.Starter;
using STP.Events;
using STP.Utils;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public class SafeArea : BaseCoreComponent {
		[NotNull] public TriggerNotifier Notifier;

		protected override void InitInternal(CoreStarter starter) {
			Notifier.OnTriggerEnter += OnPlayerEntered;
			Notifier.OnTriggerExit  += OnPlayerExit;
		}

		void OnPlayerEntered(GameObject obj) {
			var player = obj.GetComponent<Player>();
			if ( !player ) {
				return;
			}
			EventManager.Fire(new PlayerEnteredSafeArea());
		}

		void OnPlayerExit(GameObject obj) {
			var player = obj.GetComponent<Player>();
			if ( !player ) {
				return;
			}
			EventManager.Fire(new PlayerLeavedSafeArea());
		}
	}
}