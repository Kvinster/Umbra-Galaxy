using UnityEngine.UI;

using STP.Behaviour.Starter;
using STP.Common.Windows;
using STP.State;
using STP.State.Meta;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerQuestsButton : BaseMetaComponent {
        [NotNull] public Button Button;

        QuestsController      _questsController;
        StarSystemsController _starSystemsController;

        void Reset() {
            Button = GetComponent<Button>();
        }

        protected override void InitInternal(MetaStarter starter) {
            _questsController      = starter.QuestsController;
            _starSystemsController = starter.StarSystemsController;

            Button.onClick.AddListener(OnClick);
        }

        void OnClick() {
            WindowManager.Instance.Show<QuestsWindow.QuestsWindow>(x =>
                x.Init(_questsController, _starSystemsController));
        }
    }
}
