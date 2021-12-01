using UnityEngine;

using System;

using DG.Tweening;

namespace STP.Manager {
	public sealed class MinimapManager {
		const float ZoomSpeed       = 100f;
		const float MaxZoomDuration = 0.5f;
		const float StartCameraSize = 900f;
		const float MinCameraSize   = 450f;
		const float MaxCameraSize   = 2700f;

		const float ZoomRawStepSize = 10f;
		const float ZoomStepSize    = 100f;

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
			CameraSize     = 1500f;
		}

		public void ZoomIn() {
			ZoomRelative(-ZoomStepSize);
		}

		public void ZoomInRaw() {
			CameraSize -= ZoomRawStepSize;
		}

		public void ZoomOut() {
			ZoomRelative(ZoomStepSize);
		}

		public void ZoomOutRaw() {
			CameraSize += ZoomRawStepSize;
		}

		public void ResetZoom() {
			ZoomRelative(StartCameraSize - CameraSize);
		}

		void ZoomRelative(float relativeCameraSize) {
			_anim?.Kill(true);
			var endSize = Mathf.Clamp(CameraSize + relativeCameraSize, MinCameraSize, MaxCameraSize);
			relativeCameraSize = CameraSize - endSize;
			_anim = DOTween.To(() => CameraSize, x => CameraSize = x, endSize,
				Mathf.Min(Mathf.Abs(relativeCameraSize) / ZoomSpeed, MaxZoomDuration));
		}
	}
}
