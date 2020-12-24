using UnityEngine;

using Shapes;

namespace STP.Behaviour.Core {
	public sealed class ProgressBar : MonoBehaviour {
		public Rectangle Foreground;
		public Color     FullColor;
		public Color     EmptyColor;

		float _maxWidth;

		bool _isInit;

		public float Progress {
			set {
				TryInit();
				Foreground.Width = _maxWidth * Mathf.Clamp01(value);
				Foreground.Color = Color.Lerp(EmptyColor, FullColor, value);
			}
		}

		public void Init(float startProgress) {
			Progress = startProgress;
		}

		void TryInit() {
			if ( _isInit ) {
				return;
			}
			_maxWidth = Foreground.Width;
			_isInit   = true;
		}
	}
}
