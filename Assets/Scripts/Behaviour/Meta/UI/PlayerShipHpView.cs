using STP.Behaviour.Starter;
using STP.Behaviour.Utils;
using STP.State;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerShipHpView : BaseMetaComponent {
        [NotNull] public BaseProgressBar ShipHpProgressBar;

        PlayerController _playerController;

        void Reset() {
            ShipHpProgressBar = GetComponentInChildren<BaseProgressBar>();
        }

        void OnDestroy() {
            _playerController.OnShipHpChanged -= OnPlayerShipHpChanged;
        }

        protected override void InitInternal(MetaStarter starter) {
            _playerController = starter.PlayerController;
            _playerController.OnShipHpChanged += OnPlayerShipHpChanged;
            OnPlayerShipHpChanged(_playerController.ShipHp);
        }

        void OnPlayerShipHpChanged(float shipHp) {
            ShipHpProgressBar.Progress = (shipHp / PlayerState.MaxShipHp);
        }
    }
}
