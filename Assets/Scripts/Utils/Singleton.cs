namespace STP.Utils {
    public abstract class Singleton<T> where T : Singleton<T>, new() {
        static T _instance;
        public static T Instance {
            get {
                TryCreate();
                return _instance;
            }
        }

        protected virtual void Init() { }

        public static void EnsureExists() {
            TryCreate();
        }
        
        static void TryCreate() {
            if ( _instance == null ) {
                _instance = new T();
                _instance.Init();
            }
        }
    }
}