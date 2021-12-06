using UnityEngine;

using System;

namespace STP.Manager {
	public sealed class MinimapManager {
		const float StartCameraSize = 900f;
		const float MinCameraSize   = 450f;
		const float MaxCameraSize   = 2700f;

		readonly Camera _minimapCamera;

		public float CurZoom => (CameraSize / StartCameraSize);

		public float CameraSize {
			get => _minimapCamera.orthographicSize;
			private set {
				value = Mathf.Clamp(value, MinCameraSize, MaxCameraSize);
				if ( Mathf.Approximately(CameraSize, value) ) {
					return;
				}
				_minimapCamera.orthographicSize = value;
				OnCurZoomChanged?.Invoke(CurZoom);
			}
		}

		public event Action<float> OnCurZoomChanged;

		public MinimapManager(Camera minimapCamera) {
			_minimapCamera = minimapCamera;
			CameraSize     = 1500f;
		}
	}
}
