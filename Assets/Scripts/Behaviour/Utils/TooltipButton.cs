using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace STP.Behaviour.Utils {
    public sealed class TooltipButton : Button {
        public HoverTooltip Tooltip;

        protected override void OnDisable() {
            base.OnDisable();
            if ( Tooltip ) {
                Tooltip.Hide();
            }
        }

        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);
            if ( Tooltip ) {
                Tooltip.Show();
            }
        }

        public override void OnPointerExit(PointerEventData eventData) {
            base.OnPointerExit(eventData);
            if ( Tooltip ) {
                Tooltip.Hide();
            }
        }
    }
}
