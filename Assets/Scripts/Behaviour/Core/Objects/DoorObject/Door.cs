using UnityEngine;

using STP.Gameplay;
using STP.Utils;
using System;

namespace STP.Behaviour.Core.Objects.DoorObject {
    public class Door : CoreComponent {
        public float OpeningTime = 3f;
        
        DoorState _state = DoorState.Closed;
        
        Timer _timer = new Timer();
        public override void Init(CoreStarter starter) {
        } 

        void OnTriggerEnter2D(Collider2D other) {
            _state = DoorState.Opening;
             var passedTime = ( _state == DoorState.Closing ) ? (OpeningTime - _timer.TimePassed) : 0f;
            _timer.Start(OpeningTime, passedTime);
        }

        void OnTriggerExit2D(Collider2D other) {
            _state = DoorState.Closing;
            var passedTime = ( _state == DoorState.Opening ) ? (OpeningTime - _timer.TimePassed) : 0f;
            _timer.Start(OpeningTime, passedTime);
        }

        void Update() {
            if ( !_timer.DeltaTick() ) {
                return;
            }
            switch ( _state ) {
                case DoorState.Opening:
                    _state = DoorState.Opened;
                    _timer.Stop();
                    break;
                case DoorState.Closing:
                    _state = DoorState.Closed;
                    _timer.Stop();
                    break;
                case DoorState.Closed:
                case DoorState.Opened:
                        break;
                default:
                    Debug.LogError($"Unknown door state {_state}", this);
                    break;
            }
        }
    }
}