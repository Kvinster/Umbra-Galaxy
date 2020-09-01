using UnityEngine;

using System.Collections.Generic;

namespace STP.Behaviour.Core.Objects.DoorObject {
    public sealed class AutomaticDoor : BaseDoor {
        List<GameObject> _objectsInActiveArea = new List<GameObject>();
        
        void OnTriggerEnter2D(Collider2D other) {
            var baseShip = other.gameObject.GetComponent<BaseShip>();
            if ( other.isTrigger || !baseShip ) {
                return;
            }
            
            if ( !_objectsInActiveArea.Contains(other.gameObject) ) {
                _objectsInActiveArea.Add(other.gameObject);
            }

            if ( _objectsInActiveArea.Count == 1 ) {
                OpenDoor();
            }
        }

        void OnTriggerExit2D(Collider2D other) {
            var baseShip = other.gameObject.GetComponent<BaseShip>();
            if ( other.isTrigger || !baseShip ) {
                return;
            }
            
            if ( _objectsInActiveArea.Contains(other.gameObject) ) {
                _objectsInActiveArea.Remove(other.gameObject);
            }

            if ( _objectsInActiveArea.Count == 0 ) {
                CloseDoor();
            }
        }
    }
}