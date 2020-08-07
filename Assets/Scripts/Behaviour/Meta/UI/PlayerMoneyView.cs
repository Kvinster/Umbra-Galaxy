using STP.Behaviour.Starter;
using STP.State;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class PlayerMoneyView : BaseMetaComponent {
        public TMP_Text MoneyText;

        PlayerController _playerController;

        void Reset() {
            MoneyText = GetComponentInChildren<TMP_Text>();
        }

        void OnDestroy() {
            _playerController.OnMoneyChanged -= OnPlayerMoneyChanged;
        }

        protected override void InitInternal(MetaStarter starter) {
            _playerController = starter.PlayerController;
            _playerController.OnMoneyChanged += OnPlayerMoneyChanged;
            OnPlayerMoneyChanged(_playerController.Money);
        }

        void OnPlayerMoneyChanged(int curMoney) {
            UpdateText(curMoney);
        }

        void UpdateText(int money) {
            MoneyText.text = money.ToString();
        }
    }
}
