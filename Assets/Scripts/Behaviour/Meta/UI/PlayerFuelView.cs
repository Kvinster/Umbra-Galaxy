using STP.Behaviour.Starter;
using STP.State;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerFuelView : BaseMetaComponent {
        [NotNull] public TMP_Text FuelText;

        PlayerController _playerController;

        void Reset() {
            FuelText = GetComponentInChildren<TMP_Text>();
        }

        void OnDestroy() {
            _playerController.OnFuelChanged -= OnPlayerFuelChanged;
        }

        protected override void InitInternal(MetaStarter starter) {
            _playerController = starter.PlayerController;
            _playerController.OnFuelChanged += OnPlayerFuelChanged;
            OnPlayerFuelChanged(_playerController.Fuel);
        }

        void OnPlayerFuelChanged(int curFuel) {
            UpdateText(curFuel);
        }

        void UpdateText(int fuel) {
            FuelText.text = fuel.ToString();
        }
    }
}
