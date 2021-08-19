namespace STP.View.DebugGUI {
	public abstract class BaseDebugDrawable<T> : IDebugDrawable where T : class {
		protected readonly T InfoHolder;

		protected BaseDebugDrawable(T infoHolder) {
			InfoHolder = infoHolder;
		}

		public abstract void Draw();
	}
}