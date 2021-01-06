using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace STP.Behaviour.Utils {
	public sealed class PressButton : Button {
		public sealed class ButtonPressedEvent : UnityEvent { }

		public readonly ButtonPressedEvent OnPressed = new ButtonPressedEvent();

		bool _isPointerDown;

		void Update() {
			if ( _isPointerDown ) {
				OnPressed.Invoke();
			}
		}

		public override void OnPointerDown(PointerEventData eventData) {
			base.OnPointerDown(eventData);
			_isPointerDown = true;
		}

		public override void OnPointerUp(PointerEventData eventData) {
			base.OnPointerUp(eventData);
			_isPointerDown = false;
		}
	}
}
