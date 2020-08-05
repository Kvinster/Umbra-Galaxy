using UnityEngine.EventSystems;
using UnityEngine.UI;

using System;

namespace STP.Behaviour.Utils {
    public sealed class HoverButton : Button {
        public event Action OnHoverStart;
        public event Action OnHoverFinish;

        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            if ( enabled && interactable ) {
                OnHoverStart?.Invoke();
            }
        }

        public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);
            if ( enabled && interactable ) {
                OnHoverFinish?.Invoke();
            }
        }
    }
}
