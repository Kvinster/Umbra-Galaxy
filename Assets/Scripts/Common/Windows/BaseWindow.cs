using UnityEngine;
using UnityEngine.UI;

namespace STP.Common.Windows {
    public abstract class BaseWindow : MonoBehaviour {
        public Button HideButton;

        void Start() {
            if ( HideButton ) {
                HideButton.onClick.AddListener(Hide);
            }
        }
        
        protected virtual void Hide() {
            Deinit();
            WindowManager.Instance.Hide(GetType());
        }

        protected abstract void Deinit();
    }
}
