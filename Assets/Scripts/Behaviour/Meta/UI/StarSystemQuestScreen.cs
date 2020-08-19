using STP.Common;
using System;

using STP.State;
using STP.State.Meta;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Meta.UI {
    public sealed class StarSystemQuestScreen : BaseStarSystemSubScreen {
        [NotNull] public StarSystemQuestDialogManager QuestDialogManager;

        public void Init(Action hide, QuestHelper questHelper, DialogController dialogController,
            StarSystemsController starSystemsController, PlayerController playerController) {
            base.Init(hide);
            QuestDialogManager.Init(Hide, questHelper, dialogController, starSystemsController, playerController);
        }

        protected override void DeinitSpecific() {
            QuestDialogManager.Deinit();
        }

        public override void Show() {
            QuestDialogManager.Show();
        }
    }
}
