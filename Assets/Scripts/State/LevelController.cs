namespace STP.State {
    public sealed class LevelController : BaseStateController {
        readonly LevelControllerState _state = new LevelControllerState();

        public string CurQuestId => _state.CurQuestId;
    }
}
