using UnityEngine;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class MetaNewsHeadline : MonoBehaviour {
        const string NewsTemplate = "Day {0}: {1}";
        
        public TMP_Text NewsText;

        public void SetNewsText(int day, string newsText) {
            NewsText.text = string.Format(NewsTemplate, day, newsText);
        }
    }
}
