using STP.Behaviour.Starter;
using STP.State;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerFuelView : BaseMetaComponent {
        public TMP_Text FuelText;

        void Reset() {
            FuelText = GetComponentInChildren<TMP_Text>();
        }

        void OnDestroy() {
            PlayerState.Instance.OnFuelChanged -= OnPlayerFuelChanged;
        }

        protected override void InitInternal(MetaStarter starter) {
            var ps = PlayerState.Instance;
            ps.OnFuelChanged += OnPlayerFuelChanged;
            OnPlayerFuelChanged(ps.Fuel);
        }

        void OnPlayerFuelChanged(int curFuel) {
            UpdateText(curFuel);
        }

        void UpdateText(int fuel) {
            FuelText.text = fuel.ToString();
        }
    }
}
