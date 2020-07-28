using UnityEngine;

namespace STP.Common.Windows {
    public abstract class BaseWindow : MonoBehaviour {
        protected virtual void Hide() {
            Deinit();
            WindowManager.Instance.Hide(GetType());
        }

        protected abstract void Deinit();
    }
}
