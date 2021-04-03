using UnityEngine;

using STP.Behaviour.Core;
using STP.Behaviour.Starter;
using STP.Events;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Utils {
	public sealed class TransformFollower : BaseCoreComponent {
		[NotNull]
		Transform _target;
		public Vector3 Offset;

		void OnDestroy() {
			EventManager.Unsubscribe<PlayerShipChanged>(UpdatePlayerComp);
		}

		void Update() {
			if ( _target ) {
				transform.position = _target.position + Offset;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_target = starter.Player.transform;
			EventManager.Subscribe<PlayerShipChanged>(UpdatePlayerComp);
		}

		void UpdatePlayerComp(PlayerShipChanged ship) {
			_target = ship.NewPlayer.transform;
		}
	}
}