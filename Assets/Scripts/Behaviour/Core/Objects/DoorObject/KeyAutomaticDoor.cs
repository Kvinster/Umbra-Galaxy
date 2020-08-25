using UnityEngine;

using STP.Behaviour.Starter;
using STP.State;

namespace STP.Behaviour.Core.Objects.DoorObject {
    public class KeyAutomaticDoor : BaseDoor{
        public string Key;

        CorePlayerController _corePlayerController;

        protected override void InitInternal(CoreStarter starter) {
            _corePlayerController = starter.CorePlayerController;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if ( !other.gameObject.GetComponent<PlayerShip>() || !_corePlayerController.HasKey(Key) ) {
                return;
            }
            OpenDoor();
        }


        void OnTriggerStay2D(Collider2D other) {
            if ( !other.gameObject.GetComponent<PlayerShip>() || !_corePlayerController.HasKey(Key) || (State == DoorState.Opened) || (State == DoorState.Opening)) {
                return;
            }
            OpenDoor();
        }

        void OnTriggerExit2D(Collider2D other) {
            if ( !other.gameObject.GetComponent<PlayerShip>() || !_corePlayerController.HasKey(Key)  ) {
                return;
            }
            CloseDoor();
        }
    }
}