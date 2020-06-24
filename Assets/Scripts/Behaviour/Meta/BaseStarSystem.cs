using UnityEngine;
using UnityEngine.EventSystems;

using STP.Behaviour.Starter;
using STP.Utils.PropertyAttribute;

using TMPro;

namespace STP.Behaviour.Meta {
    public abstract class BaseStarSystem : BaseMetaComponent {
        [StarSystemName]
        public string Name;
        [Space]
        public TMP_Text     StarSystemNameText;
        public EventTrigger EventTrigger;

        protected PlayerShip PlayerShip;

        protected void OnValidate() {
#if UNITY_EDITOR
            if ( UnityEditor.PrefabUtility.IsPartOfPrefabInstance(this) ) {
                gameObject.name = Name;
                if ( StarSystemNameText && !string.IsNullOrEmpty(gameObject.scene.name) &&
                     (StarSystemNameText.text != Name) ) {
                    StarSystemNameText.text = Name;
                    UnityEditor.EditorUtility.SetDirty(StarSystemNameText);
                }
            }
#endif
        }
        
        protected sealed override void InitInternal(MetaStarter starter) {
            PlayerShip = starter.PlayerShip;
            
            if ( EventTrigger ) {
                var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
                entry.callback.AddListener(_ => OnClick());
                EventTrigger.triggers.Add(entry);
            }

            starter.StarSystemsManager.RegisterStarSystem(this);

            InitSpecific(starter);
        }

        protected abstract void InitSpecific(MetaStarter starter);

        protected virtual void OnClick() {
            if ( PlayerShip.TryMoveTo(this, out var promise) ) {
                promise.Then(OnPlayerArrive);
            }
        }
        
        protected virtual void OnPlayerArrive() { }
    }
}
