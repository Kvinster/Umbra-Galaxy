using UnityEngine;
using UnityEngine.UI;

namespace STP.Behaviour.Utils.ProgressBar {
	public sealed class ProgressBarUi : BaseProgressBar {
		public Image Foreground;
		public Color FullColor  = Color.white;
		public Color EmptyColor = Color.white;

		public override float Progress {
			set {
				value                 = Mathf.Clamp01(value);
				Foreground.fillAmount = value;
				Foreground.color      = Color.Lerp(EmptyColor, FullColor, value);
			}
		}

		protected override void CheckDescription() {
			if ( Foreground && (Foreground.type != Image.Type.Filled) ) {
				Debug.LogErrorFormat(this, "{0}.{1}: {2} image must have type Filled", nameof(ProgressBarUi),
					nameof(CheckDescription), nameof(Foreground));
			}
		}

		public override void Init(float startProgress) {
			Progress = startProgress;
		}
	}
}
