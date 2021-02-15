using UnityEngine;

namespace STP.Behaviour.Core.Enemy {
	public abstract class BaseControllableEnemy : BaseEnemy {
		public abstract void SetTarget(Transform target);
	}
}