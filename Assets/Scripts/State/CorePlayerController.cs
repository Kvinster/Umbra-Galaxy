namespace STP.State {
    public class CorePlayerController : BaseStateController {
        readonly CorePlayerControllerState _state = new CorePlayerControllerState();

        public void ResetState() {
            _state.Reset();
        }

        public void AddKey(string key) {
            _state.Keys.Add(key);
        }

        public bool HasKey(string key) {
            return _state.Keys.Contains(key);
        }
    }
}