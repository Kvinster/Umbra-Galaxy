using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class MetaNewsHeadline : GameComponent {
        const string NewsTemplate = "Day {0}: {1}";
        
        [NotNull] public TMP_Text NewsText;

        public void SetNewsText(int day, string newsText) {
            NewsText.text = string.Format(NewsTemplate, day, newsText);
        }
    }
}
