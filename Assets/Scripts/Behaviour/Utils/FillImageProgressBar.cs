using UnityEngine;
using UnityEngine.UI;

using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Utils {
    public sealed class FillImageProgressBar : BaseProgressBar {
        [NotNull] public Image Foreground;
        
        float _progress;

        public float Progress {
            get => _progress;
            set {
                _progress = Mathf.Clamp01(value);
                UpdateView();
            }
        }

        protected override void CheckDescription() {
            if ( Foreground && (Foreground.type != Image.Type.Filled) ) {
                Debug.LogError("Foreground image must have type Filled", this);
            }
        }
        
        void Start() {
            UpdateView();
        }


        protected override void UpdateView() {
            Foreground.fillAmount = Progress;
        }
    }
}
