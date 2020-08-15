using UnityEngine.UI;

using System;

using STP.Utils;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class StarSystemQuestDialogChoiceView : GameBehaviour {
        public TMP_Text ChoiceText;
        public Button   Button;

        int _index;

        public event Action<int> OnClick;

        void Reset() {
            ChoiceText = GetComponentInChildren<TMP_Text>();
            Button     = GetComponentInChildren<Button>();
        }

        public void CommonInit(int index) {
            _index = index;

            Button.onClick.AddListener(() => OnClick?.Invoke(_index));
        }

        public void Deinit() {
            Button.onClick.RemoveAllListeners();
        }

        public void SetText(string choiceText) {
            ChoiceText.text = choiceText;
        }
    }
}
