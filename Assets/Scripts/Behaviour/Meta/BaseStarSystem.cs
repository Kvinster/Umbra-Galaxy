using UnityEngine.EventSystems;

using STP.Behaviour.Starter;
using STP.Common;
using TMPro;

namespace STP.Behaviour.Meta {
    public abstract class BaseStarSystem : BaseMetaComponent {
        public TMP_Text     StarSystemNameText;
        public EventTrigger EventTrigger;

        protected PlayerShip PlayerShip;
        
        public abstract string Id { get; }
        
        public abstract StarSystemType Type { get; }

        public override bool HighPriorityInit => true;

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
        
        protected virtual void OnPlayerArrive(bool success) { }
    }
}
