namespace STP.Behaviour.Utils.ProgressBar {
	public interface IProgressBar {
		float Progress { set; }

		void Init(float startProgress);
	}
}
