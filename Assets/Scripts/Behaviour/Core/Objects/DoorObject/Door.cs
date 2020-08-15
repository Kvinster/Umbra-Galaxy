﻿using UnityEngine;

using STP.Gameplay;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Objects.DoorObject {
    public class Door : CoreComponent {
        public float OpeningTime = 3f;
        
        [NotNull] public DoorFrame LeftFrame;
        [NotNull] public DoorFrame RightFrame;
        
        readonly Timer _timer = new Timer();
        
        DoorState _state = DoorState.Closed;

        DoorState State {
            get => _state;
            set {
                LeftFrame.SetDoorState(value);
                LeftFrame.SetProgress(_timer.NormalizedProgress);
                
                RightFrame.SetDoorState(value);
                RightFrame.SetProgress(_timer.NormalizedProgress);
                
                _state = value;
            }
        }

        public override void Init(CoreStarter starter) {
            LeftFrame.Init();
            RightFrame.Init();
        } 

        void OnTriggerEnter2D(Collider2D other) {
            if ( !other.gameObject.GetComponent<PlayerShip>() ) {
                return;
            }
            var passedTime = ( State == DoorState.Closing ) ? _timer.LeftTime : 0f;
            State = DoorState.Opening;
            _timer.Start(OpeningTime, passedTime);
        }

        void OnTriggerExit2D(Collider2D other) {
            if ( !other.gameObject.GetComponent<PlayerShip>() ) {
                return;
            }
            var passedTime = ( State == DoorState.Opening ) ? _timer.LeftTime : 0f;
            State = DoorState.Closing;
            _timer.Start(OpeningTime, passedTime);
        }

        void Update() {
            if ( _timer.DeltaTick() ) {
                switch ( State ) {
                    case DoorState.Opening:
                        State = DoorState.Opened;
                        _timer.Stop();
                        break;
                    case DoorState.Closing:
                        State = DoorState.Closed;
                        _timer.Stop();
                        break;
                    case DoorState.Closed:
                    case DoorState.Opened:
                        break;
                    default:
                        Debug.LogError($"Unknown door state {State}", this);
                        break;
                }
            }
            else {
                LeftFrame.SetProgress(_timer.NormalizedProgress);
                RightFrame.SetProgress(_timer.NormalizedProgress);
            }
        }
    }
}