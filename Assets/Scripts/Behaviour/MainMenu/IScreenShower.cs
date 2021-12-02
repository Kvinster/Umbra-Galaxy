namespace STP.Behaviour.MainMenu {
	public interface IScreenShower {
		void Show<T>() where T : class, IScreen;
	}
}