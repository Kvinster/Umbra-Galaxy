using System;

using STP.State;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Meta.UI {
    public sealed class StarSystemQuestScreen : BaseStarSystemSubScreen {
        [NotNull] public StarSystemQuestDialogManager QuestDialogManager;

        public void Init(Action hide, DialogController dialogController, QuestsController questsController,
            PlayerController playerController) {
            base.Init(hide);
            QuestDialogManager.Init(Hide, dialogController, questsController, playerController);
        }

        protected override void DeinitSpecific() {
            QuestDialogManager.Deinit();
        }

        public override void Show() {
            QuestDialogManager.Show();
        }
    }
}
