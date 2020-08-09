namespace STP.View.DebugGUI {
    public abstract class BaseMenu<T, TT> : IDebugDrawable {
        protected T  MenuHolder;
        protected TT InfoHolder;

        public BaseMenu(T menuHolder, TT infoHolder) {
            MenuHolder = menuHolder;
            InfoHolder = infoHolder;
        }
		
        public abstract void Draw();
    }
}