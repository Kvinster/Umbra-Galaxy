using UnityEngine;

using STP.Behaviour.Starter;
using STP.Controller;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.PowerUps {
	public abstract class BasePowerUp : BaseCoreComponent {
		[NotNull] public TriggerNotifier Notifier;

		protected PlayerManager    PlayerManager;
		protected PlayerController PlayerController;

		protected void Reset() {
			Notifier = GetComponentInChildren<TriggerNotifier>();
		}

		protected override void InitInternal(CoreStarter starter) {
			PlayerManager    = starter.PlayerManager;
			PlayerController = starter.PlayerController;

			Notifier.OnTriggerEnter += OnRangeEnter;
		}

		protected void OnDestroy() {
			Notifier.OnTriggerEnter -= OnRangeEnter;
		}

		protected virtual void OnRangeEnter(GameObject go) {
			var playerComp = go.GetComponent<Player>();
			if ( !playerComp ) {
				return;
			}
			OnPlayerEnter();
			Destroy(gameObject);
		}

		protected abstract void OnPlayerEnter();
	}
}