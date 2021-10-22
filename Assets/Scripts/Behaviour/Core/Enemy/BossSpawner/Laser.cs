using System;
using STP.Behaviour.Core;
using UnityEngine;

using STP.Utils;

namespace STP.Gameplay.Weapon.LaserWeapon {
    
    public enum WeaponState {
        Idle,
        Charge,
        Charged,
        Fire
    }
    
    public sealed class Laser {
        const float MaxDistance     = 10000f;

        float _laserDamage;
        
        WeaponState _state;

        readonly Transform  _mountTrans;
        readonly Collider2D _ownerCollider;

        readonly RaycastHit2D[] _hits = new RaycastHit2D[10];

        public float CurHitDistance { get; private set; }
        
        
        public event Action<WeaponState> StateChanged;
        
        public WeaponState CurState {
            get => _state;
            private set {
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }
        
        public Laser(Transform mountTrans, Collider2D ownerCollider, float damage) {
            _mountTrans    = mountTrans;
            _ownerCollider = ownerCollider;
            _laserDamage   = damage;
        }

        public void TryShoot() {
            if ( CurState != WeaponState.Fire ) {
                CurState = WeaponState.Fire;
            }
        }

        public void TryStopShoot() {
            CurState = WeaponState.Charged;
        }

        public void Update() {
            CurHitDistance = MaxDistance;
            if ( CurState != WeaponState.Fire ) {
                return;
            }
            if ( !TryRaycast(out var hit) ) {
                return;
            }
            CurHitDistance = hit.distance;
            var ship = hit.collider.GetComponent<IDestructible>();
            ship?.TakeDamage(_laserDamage * Time.deltaTime);
        }

        bool TryRaycast(out RaycastHit2D hit) {
            var hitsCount = Physics2D.RaycastNonAlloc(_mountTrans.position,
                _mountTrans.TransformDirection(_mountTrans.localRotation * Vector2.up), _hits, MaxDistance);
            var hitIndex = FindNearestHitIndex(_hits, hitsCount);
            if ( hitIndex != -1 ) {
                hit = _hits[hitIndex];
                return true;
            }
            hit = default;
            return false;
        }

        int FindNearestHitIndex(RaycastHit2D[] hits, int realHitsCount) {
            var minDistance = float.MaxValue;
            var minHitIndex = -1;
            for ( var i = 0; i < realHitsCount; i++ ) {
                var hit = hits[i];
                if ( !hit.collider.isTrigger && (hit.distance < minDistance) && (hit.collider != _ownerCollider) ) {
                    minDistance = hit.distance;
                    minHitIndex = i;
                }
            }
            return minHitIndex;
        }
    }
}