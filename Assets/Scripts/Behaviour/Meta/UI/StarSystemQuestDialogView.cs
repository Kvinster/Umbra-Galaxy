using System;
using System.Collections.Generic;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class StarSystemQuestDialogView : GameBehaviour {
        [NotNull]
        public TMP_Text DialogText;
        [NotNullOrEmpty]
        public List<StarSystemQuestDialogChoiceView> ChoiceViews = new List<StarSystemQuestDialogChoiceView>();

        public event Action<int> OnChoiceClick;

        public void CommonInit() {
            for ( var i = 0; i < ChoiceViews.Count; i++ ) {
                var choiceView = ChoiceViews[i];
                choiceView.CommonInit(i);
                choiceView.OnClick += OnChoiceClickInternal;
            }
        }

        public void Deinit() {
            foreach ( var choiceView in ChoiceViews ) {
                choiceView.Deinit();
                choiceView.OnClick -= OnChoiceClickInternal;
            }
        }

        public void UpdateView(string dialogText, List<string> choiceTexts) {
            DialogText.text = dialogText;
            var choiceViewIndex = 0;
            foreach ( var choiceText in choiceTexts ) {
                var choiceView = ChoiceViews[choiceViewIndex++];
                choiceView.SetText(choiceText);
                choiceView.gameObject.SetActive(true);
            }
            for ( ; choiceViewIndex < ChoiceViews.Count; ++choiceViewIndex ) {
                ChoiceViews[choiceViewIndex].gameObject.SetActive(false);
            }
        }

        void OnChoiceClickInternal(int choiceIndex) {
            OnChoiceClick?.Invoke(choiceIndex);
        }
    }
}
