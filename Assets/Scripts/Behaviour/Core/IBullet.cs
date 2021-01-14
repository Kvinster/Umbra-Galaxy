using UnityEngine;

using STP.Behaviour.Starter;

namespace STP.Behaviour.Core {
	public interface IBullet {
		void Init(float damage, Vector2 force, float rotation, params Collider2D[] ownerColliders);
	}
}