using UnityEngine.EventSystems;

using STP.Behaviour.Starter;

using TMPro;

namespace STP.Behaviour.Meta {
    public sealed class DummyStarSystem : BaseStarSystem {
        public TMP_Text StarSystemNameText;
        
        EventTrigger _eventTrigger;

        void OnValidate() {
#if UNITY_EDITOR
            gameObject.name = Name;
            if ( StarSystemNameText && (StarSystemNameText.text != Name) ) {
                StarSystemNameText.text = Name;
                UnityEditor.EditorUtility.SetDirty(StarSystemNameText);
            }
#endif
        }
        
        protected override void InitInternal(MetaStarter starter) {
            _eventTrigger = GetComponent<EventTrigger>();
            if ( _eventTrigger ) {
                var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
                entry.callback.AddListener(_ => starter.PlayerShip.TryMoveTo(this));
                _eventTrigger.triggers.Add(entry);
            }
        }
    }
}
