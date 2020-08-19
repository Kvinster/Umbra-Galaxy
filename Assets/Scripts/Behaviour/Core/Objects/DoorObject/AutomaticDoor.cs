using UnityEngine;

using STP.Gameplay;

namespace STP.Behaviour.Core.Objects.DoorObject {
    public class AutomaticDoor : BaseDoor {
        protected override void InitInternal(CoreStarter starter) { }
        
        void OnTriggerEnter2D(Collider2D other) {
            if ( !other.gameObject.GetComponent<PlayerShip>() ) {
                return;
            }
            OpenDoor();
        }

        void OnTriggerExit2D(Collider2D other) {
            if ( !other.gameObject.GetComponent<PlayerShip>() ) {
                return;
            }
            CloseDoor();
        }
    }
}