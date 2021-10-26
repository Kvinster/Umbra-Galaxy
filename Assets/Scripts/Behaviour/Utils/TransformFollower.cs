using UnityEngine;

using STP.Behaviour.Core;
using STP.Behaviour.Starter;

namespace STP.Behaviour.Utils {
	public sealed class TransformFollower : BaseCoreComponent {
		public Vector3 Offset;

		Transform _target;

		void Update() {
			if ( _target ) {
				transform.position = _target.position + Offset;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_target = starter.Player.transform;
		}
	}
}