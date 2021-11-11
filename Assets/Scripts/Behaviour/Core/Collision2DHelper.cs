using UnityEngine;

namespace STP.Behaviour.Core {
	public static class Collision2DHelper {
		public static bool TryTakeDamage(this Collision2D collision, float damage) {
			if ( !collision.gameObject ) {
				return false;
			}
			if ( collision.collider.gameObject != collision.gameObject ) {
				return false;
			}
			if ( collision.gameObject.TryGetComponent<IDestructible>(out var destructible) ) {
				destructible.TakeDamage(damage);
				return true;
			}
			return false;
		}
	}
}
