﻿using UnityEngine;

namespace STP.State.Core {
    public class CoreShipState {
        public Vector2 Position;
        public Vector2 Velocity;
        
        public float Hp;

        public CoreShipState(PlayerShipState playerPlayerState) {
            Hp = playerPlayerState.Hp;
        }

        public CoreShipState(int hp) {
            Hp = hp;
        }
    }
}