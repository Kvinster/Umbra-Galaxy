using UnityEngine;

namespace STP.Behaviour.Core.Objects.DoorObject {
    public sealed class AutomaticDoor : BaseDoor {
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