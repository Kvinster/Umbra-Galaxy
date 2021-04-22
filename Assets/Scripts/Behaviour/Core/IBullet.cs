using UnityEngine;

namespace STP.Behaviour.Core {
	public interface IBullet {
		void Init(float damage, float speed, params Collider2D[] ownerColliders);
	}
}