using UnityEngine;

using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.LaserWeapon {
    public class LaserWeaponView : BaseWeaponView {
        public Beam Beam;
        BaseWeapon _weapon;
        Collider2D _ownerCollider;
        
        RaycastHit2D[] _hits = new RaycastHit2D[10];
        
        public override void Init(CoreStarter starter, BaseShip ownerShip, BaseWeapon ownerWeapon) {
            if ( !(ownerWeapon is Laser laserWeapon) ) {
                return;
            }
            _weapon               = laserWeapon;
            _ownerCollider        = ownerShip.GetComponent<Collider2D>();
            _weapon.StateChanged += OnWeaponStateChanged; 
            OnWeaponStateChanged(_weapon.CurState);
        }

        void OnWeaponStateChanged(WeaponState newWeaponState) {
            Beam.gameObject.SetActive(newWeaponState == WeaponState.FIRE);
        }

        void OnDestroy() {
            _weapon.StateChanged -= OnWeaponStateChanged;
        }

        void Update() {
            if ( _weapon.CurState == WeaponState.FIRE ) {
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