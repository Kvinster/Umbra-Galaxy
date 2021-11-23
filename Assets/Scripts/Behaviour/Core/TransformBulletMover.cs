using STP.Utils;
using UnityEngine;

namespace STP.Behaviour.Core {
	public class TransformBulletMover : GameComponent {
		public Vector3 Velocity;
		
		protected void Update() {
			transform.position += Velocity * Time.deltaTime;
		}
	}
}