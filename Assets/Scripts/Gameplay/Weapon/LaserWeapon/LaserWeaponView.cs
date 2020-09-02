using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Utils.GameComponentAttributes;

namespace STP.Gameplay.Weapon.LaserWeapon {
    public sealed class LaserWeaponView : BaseWeaponView<Laser> {
        [NotNull] public Beam Beam;

        Collider2D _ownerCollider;

        RaycastHit2D[] _hits = new RaycastHit2D[10];

        protected override void Init(CoreStarter starter, BaseShip ownerShip) {
            _ownerCollider       = ownerShip.GetComponent<Collider2D>();
            Weapon.StateChanged += OnWeaponStateChanged;
            OnWeaponStateChanged(Weapon.CurState);
        }

        void OnWeaponStateChanged(WeaponState newWeaponState) {
            Beam.gameObject.SetActive(newWeaponState == WeaponState.Fire);
        }

        void OnDestroy() {
            Weapon.StateChanged -= OnWeaponStateChanged;
        }

        void Update() {
            if ( Weapon.CurState == WeaponState.Fire ) {
                var minDistanceHit = DoRaycast();
                if ( minDistanceHit != -1 ) {
                    Beam.SetLength (_hits[minDistanceHit].distance);
                    Beam.DealDamage(_hits[minDistanceHit].collider);
                }
                else {
                    Beam.SetLength(1000000);
                }
            }
        }

        int DoRaycast() {
            var hitsCount = Physics2D.RaycastNonAlloc(Beam.transform.position,
                Beam.transform.TransformDirection(Beam.transform.localRotation * Vector2.up), _hits, 1000000);
            var minDistance = float.MaxValue;
            var minDistanceHit = -1;
            for ( var i = 0; i < hitsCount; i++ ) {
                var hit = _hits[i];
                if ( hit.collider && !hit.collider.isTrigger && hit.distance < minDistance &&
                     (hit.collider != _ownerCollider) ) {
                    minDistance = hit.distance;
                    minDistanceHit = i;
                }
            }
            return minDistanceHit;
        }

        void OnDrawGizmos() {
            Gizmos.DrawRay(Beam.transform.position, Beam.transform.TransformDirection(Beam.transform.localRotation * Vector2.up) * 10000);
        }
    }
}