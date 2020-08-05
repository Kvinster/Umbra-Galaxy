using STP.Behaviour.Starter;

namespace STP.Behaviour.Meta.UI {
    public sealed class MetaUiManager : BaseMetaComponent {
        public MetaDebugInfoText DebugInfoText;
        public MetaNewsBlock     NewsBlock;
        public GameOverScreen    GameOverScreen;
        
        protected override void InitInternal(MetaStarter starter) {
            DebugInfoText.Init(starter);
            DebugInfoText.gameObject.SetActive(true);

            NewsBlock.Init(starter.TimeManager);
            NewsBlock.gameObject.SetActive(true);

            GameOverScreen.CommonInit(this);
            GameOverScreen.gameObject.SetActive(false);
        }

        public void ShowGameOverScreen() {
            GameOverScreen.gameObject.SetActive(true);
        }
    }
}
