using UnityEngine;
using UnityEngine.UI;

using System;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Meta.UI {
    public abstract class BaseStarSystemSubScreen : GameComponent {
        Action _hide;
        
        void OnEnable() {
            Debug.Assert(_hide != null, "_hide != null");
        }
        
        public abstract void Show();

        public void Deinit() {
            _hide = null;
            
            DeinitSpecific();
        }

        protected void Init(Action hide) {
            _hide = hide;
        }

        protected void Hide() {
            HideSpecific();
            _hide.Invoke();
        }
        
        protected virtual void HideSpecific() { }
        
        protected abstract void DeinitSpecific();
    }
}
