using UnityEngine;

using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.LaserWeapon {
    public class Laser : BaseWeapon {
        const float DamagePerSecond = 2f; // temporary
        const float MaxDistance     = 10000f;

        public override WeaponType Name => WeaponType.Laser;

        readonly Transform  _mountTrans;
        readonly Collider2D _ownerCollider;

        readonly RaycastHit2D[] _hits = new RaycastHit2D[10];

        public float CurHitDistance { get; private set; }

        public Laser(Transform mountTrans, Collider2D ownerCollider) {
            _mountTrans    = mountTrans;
            _ownerCollider = ownerCollider;
        }

        public void TryShoot() {
            if ( CurState != WeaponState.Fire ) {
                CurState = WeaponState.Fire;
            }
        }

        public void TryStopShoot() {
            CurState = WeaponState.Charged;
        }

        protected override void Update(float passedTime) {
            CurHitDistance = MaxDistance;

            if ( CurState == WeaponState.Fire ) {
                if ( TryRaycast(out var hit) ) {
                    CurHitDistance = hit.distance;

                    var ship = hit.collider.GetComponent<IDestructable>();
                    ship?.GetDamage(DamagePerSecond * passedTime);
                }
            }
        }

        bool TryRaycast(out RaycastHit2D hit) {
            var hitsCount = Physics2D.RaycastNonAlloc(_mountTrans.position,
                _mountTrans.TransformDirection(_mountTrans.localRotation * Vector2.up), _hits, MaxDistance);
            var minDistance = float.MaxValue;
            var minHitIndex = -1;
            for ( var i = 0; i < hitsCount; i++ ) {
                var tmpHit = _hits[i];
                if ( tmpHit.collider && !tmpHit.collider.isTrigger && (tmpHit.distance < minDistance) &&
                     (tmpHit.collider != _ownerCollider) ) {
                    minDistance = tmpHit.distance;
                    minHitIndex = i;
                }
            }
            if ( minHitIndex != -1 ) {
                hit = _hits[minHitIndex];
                return true;
            }
            hit = default;
            return false;
        }
    }
}