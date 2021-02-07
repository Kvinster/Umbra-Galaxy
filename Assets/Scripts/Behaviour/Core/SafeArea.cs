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
			Notifier.OnTriggerEnter += OnObjectEnter;
			Notifier.OnTriggerExit  += OnObjectExit;

			var notifierCollider = Notifier.Collider;
			if ( notifierCollider && notifierCollider.OverlapPoint(starter.PlayerStartPos.position) ) {
				OnPlayerEnter();
			}
		}

		void OnObjectEnter(GameObject obj) {
			var player = obj.GetComponent<Player>();
			if ( !player ) {
				return;
			}
			OnPlayerEnter();
		}

		void OnPlayerEnter() {
			EventManager.Fire(new PlayerEnteredSafeArea());
		}

		void OnObjectExit(GameObject obj) {
			var player = obj.GetComponent<Player>();
			if ( !player ) {
				return;
			}
			OnPlayerExit();
		}

		void OnPlayerExit() {
			EventManager.Fire(new PlayerLeftSafeArea());
		}
	}
}