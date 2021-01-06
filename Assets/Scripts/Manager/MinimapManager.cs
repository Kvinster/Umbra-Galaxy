using UnityEngine;

using System;

using DG.Tweening;

namespace STP.Manager {
	public sealed class MinimapManager {
		const float ZoomSpeed       = 100f;
		const float StartCameraSize = 900f;
		const float MinCameraSize   = 450f;
		const float MaxCameraSize   = 2700f;

		const float ZoomStepSize = 100f;

		readonly Camera _minimapCamera;

		Tween _anim;

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
		}

		public void ZoomIn() {
			ZoomRelative(-ZoomStepSize);
		}

		public void ZoomOut() {
			ZoomRelative(ZoomStepSize);
		}

		void ZoomRelative(float relativeCameraSize) {
			_anim?.Kill(true);
			_anim = DOTween.To(() => CameraSize, x => CameraSize = x, CameraSize + relativeCameraSize,
				Mathf.Abs(relativeCameraSize) / ZoomSpeed);
		}
	}
}
