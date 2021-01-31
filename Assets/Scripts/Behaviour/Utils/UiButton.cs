using UnityEngine.EventSystems;
using UnityEngine.UI;

using STP.Behaviour.Sound;

namespace STP.Behaviour.Utils {
	public sealed class UiButton : Button {
		public override void OnPointerClick(PointerEventData eventData) {
			base.OnPointerClick(eventData);
			UiAudioPlayer.Instance.PlayUiClick();
		}
	}
}
