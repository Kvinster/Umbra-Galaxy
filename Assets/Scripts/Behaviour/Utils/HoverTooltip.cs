using UnityEngine;
using UnityEngine.EventSystems;

namespace STP.Behaviour.Utils {
    public sealed class HoverTooltip : MonoBehaviour {
        public EventTrigger EventTrigger;
        public GameObject   Root;

        bool _showInt;
        bool _showExt;

        bool ShowInt {
            get => _showInt;
            set {
                _showInt = value;
                SetShown(_showInt || _showExt);
            }
        }

        bool ShowExt {
            get => _showExt;
            set {
                _showExt = value;
                SetShown(_showInt || _showExt);
            }
        }

        void Start() {
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            entry.callback.AddListener(_ => ShowInt = true);
            EventTrigger.triggers.Add(entry);

            entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            entry.callback.AddListener(_ => ShowInt = false);
            EventTrigger.triggers.Add(entry);

            Root.SetActive(false);
        }

        public void Show() {
            ShowExt = true;
        }

        public void Hide() {
            ShowExt = false;
        }

        void SetShown(bool isShown) {
            Root.SetActive(isShown);
        }
    }
}
