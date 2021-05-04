using STP.Utils;

namespace STP.Behaviour.Utils.ProgressBar {
	public abstract class BaseProgressBar : GameComponent, IProgressBar {
		public abstract float Progress { set; }

		public abstract void Init(float startProgress);
	}
}
