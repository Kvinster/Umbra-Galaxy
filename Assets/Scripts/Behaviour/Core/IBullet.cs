using UnityEngine;

namespace STP.Behaviour.Core {
	public interface IBullet {
		void Init(float damage, float speed, float rotation, params Collider2D[] ownerColliders);
	}
}