using UnityEngine;

namespace STP.Behaviour.Core.Enemy {
	public interface IVisibleHandler {
		void OnBecomeVisibleForPlayer(Transform playerTransform);
		void OnBecomeInvisibleForPlayer();
	}
}