using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Utils {
	public sealed class CameraUtility : SingleBehaviour<CameraUtility> {
		Camera _camera;

		public Camera Camera {
			get {
				TryFixCamera();
				return _camera;
			}
		}

		void Update() {
			TryFixCamera();
		}

		protected override void Init() {
			TryFixCamera();
		}

		void TryFixCamera() {
			if ( _camera ) {
				return;
			}
			_camera = Camera.main;
		}
	}
}
