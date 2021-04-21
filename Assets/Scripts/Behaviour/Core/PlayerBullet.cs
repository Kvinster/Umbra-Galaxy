using UnityEngine;

namespace STP.Behaviour.Core {
	public sealed class PlayerBullet : Bullet {
		bool _isOutOfPlayerTrigger;

		public override bool NeedToDestroy => base.NeedToDestroy || _isOutOfPlayerTrigger;

		public void OnTriggerExit2D(Collider2D other) {
			var obj = other.GetComponent<EnemiesTrigger>();
			if ( obj ) {
				_isOutOfPlayerTrigger = true;
			}
		}
	}
}
