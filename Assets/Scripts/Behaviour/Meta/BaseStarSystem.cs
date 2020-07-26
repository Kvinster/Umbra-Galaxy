using UnityEngine.EventSystems;

using STP.Behaviour.Starter;
using STP.Common;

using TMPro;

namespace STP.Behaviour.Meta {
    public abstract class BaseStarSystem : BaseMetaComponent {
        public TMP_Text     StarSystemNameText;
        public EventTrigger EventTrigger;

        protected PlayerShipMovementController PlayerShipMovementController;
        
        public abstract string Id { get; }
        
        public abstract StarSystemType Type { get; }

        public override bool HighPriorityInit => true;

        public virtual bool InterruptOnPlayerArriveIntermediate => false;
        
        public virtual void OnPlayerArrive(bool success) { }

        protected sealed override void InitInternal(MetaStarter starter) {
            PlayerShipMovementController = starter.PlayerShip.MovementController;
            
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
            switch ( PlayerShipMovementController.CurState ) {
                case PlayerShipMovementController.State.Idle: {
                    PlayerShipMovementController.TrySelect(this);
                    break;
                }
                case PlayerShipMovementController.State.Selected: {
                    if ( PlayerShipMovementController.DestSystem == this ) {
                        PlayerShipMovementController.Move().Then(OnPlayerArrive);
                    } else {
                        PlayerShipMovementController.TrySelect(this);
                    }
                    break;
                }
            }
        }
    }
}
