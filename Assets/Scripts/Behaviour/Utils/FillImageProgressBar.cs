using UnityEngine;
using UnityEngine.UI;

using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Utils {
    public sealed class FillImageProgressBar : BaseProgressBar {
        [NotNull] public Image Foreground;

        protected override void CheckDescription() {
            if ( Foreground && (Foreground.type != Image.Type.Filled) ) {
                Debug.LogError("Foreground image must have type Filled", this);
            }
        }

        protected override void UpdateView() {
            Foreground.fillAmount = Progress;
        }
    }
}
