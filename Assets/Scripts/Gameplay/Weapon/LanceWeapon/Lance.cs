using UnityEngine;

using STP.Gameplay.Weapon.Chargeable;
using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.LanceWeapon {
    public class Lance : ChargeableWeapon  {
        public const float MaxDistance = 10000f;

        public override WeaponType Name         => WeaponType.Lance;
        public override float      Damage       => 5f;
        protected override float   ChargingTime => 1f;

        readonly Transform  _mountTrans;
        readonly Collider2D _ownerCollider;

        readonly RaycastHit2D[] _hits = new RaycastHit2D[10];

        public Lance(Transform mountTrans, Collider2D ownerCollider) {
            _mountTrans    = mountTrans;
            _ownerCollider = ownerCollider;
        }

        public override void ReleaseCharging() {
            base.ReleaseCharging();

            if ( CurState == WeaponState.Fire ) {
                TryShoot();
            }
        }

        void TryShoot() {
            var hitsCount = Physics2D.RaycastNonAlloc(_mountTrans.position,
                _mountTrans.TransformDirection(_mountTrans.localRotation * Vector2.up), _hits, MaxDistance);
            for ( var i = 0; i < hitsCount; i++ ) {
                var hit = _hits[i];
                if ( hit.collider && !hit.collider.isTrigger && (hit.collider != _ownerCollider) ) {
                    var ship = hit.collider.GetComponent<IDestructable>();
                    ship?.GetDamage(Damage);
                }
            }
        }
    }
}