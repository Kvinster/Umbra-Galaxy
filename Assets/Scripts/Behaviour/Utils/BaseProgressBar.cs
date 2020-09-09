using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Utils {
    public abstract class BaseProgressBar : GameComponent {
        // ReSharper disable once InconsistentNaming
        protected float _progress;

        public float Progress {
            get => _progress;
            set {
                _progress = Mathf.Clamp01(value);
                UpdateView();
            }
        }

        protected virtual void Start() {
            UpdateView();
        }

        protected abstract void UpdateView();
    }
}
