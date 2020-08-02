using STP.Behaviour.Starter;
using STP.State;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerMoneyView : BaseMetaComponent {
        public TMP_Text MoneyText;

        void Reset() {
            MoneyText = GetComponentInChildren<TMP_Text>();
        }

        void OnDestroy() {
            PlayerState.Instance.OnMoneyChanged -= OnPlayerMoneyChanged;
        }

        protected override void InitInternal(MetaStarter starter) {
            var ps = PlayerState.Instance;
            ps.OnMoneyChanged += OnPlayerMoneyChanged;
            OnPlayerMoneyChanged(ps.Money);
        }

        void OnPlayerMoneyChanged(int curMoney) {
            UpdateText(curMoney);
        }

        void UpdateText(int money) {
            MoneyText.text = money.ToString();
        }
    }
}
