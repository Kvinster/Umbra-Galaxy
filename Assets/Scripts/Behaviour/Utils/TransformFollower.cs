using UnityEngine;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Utils {
	public sealed class TransformFollower : GameComponent {
		[NotNull]
		public Transform Target;
		public Vector3 Offset;

		void Update() {
			if ( Target ) {
				transform.position = Target.position + Offset;
			}
		}
	}
}
