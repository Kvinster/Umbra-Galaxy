using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Utils;

namespace STP.Gameplay.Weapon.LanceWeapon {
    public class LanceWeaponView : BaseWeaponView<Lance> {
        public Beam       Beam;
        public GameObject ChargingImage;

        readonly Timer _timer = new Timer();

        Collider2D  _ownerCollider;

        readonly RaycastHit2D[] _hits = new RaycastHit2D[10];

        protected override void Init(CoreStarter starter, BaseShip ownerShip) {
            _ownerCollider = ownerShip.GetComponent<Collider2D>();
            Weapon.StateChanged += OnWeaponStateChanged;
            OnWeaponStateChanged(Weapon.CurState);
        }

        void OnWeaponStateChanged(WeaponState newWeaponState) {
            ChargingImage.SetActive(newWeaponState == WeaponState.Charge || newWeaponState == WeaponState.Charged);
            if ( newWeaponState == WeaponState.Fire ) {
                Beam.gameObject.SetActive(true);
                _timer.Start(0.2f);
            }
        }

        void OnDestroy() {
            Weapon.StateChanged -= OnWeaponStateChanged;
        }

        void Update() {
            if ( _timer.Tick(Time.deltaTime) ) {
                Beam.gameObject.SetActive(false);
                _timer.Stop();
            }
            if ( Weapon.CurState == WeaponState.Fire ) {
                DoRaycast();
                Beam.SetLength(1000000);
            }
        }

        void DoRaycast() {
            var hitsCount = Physics2D.RaycastNonAlloc(Beam.transform.position,
                Beam.transform.TransformDirection(Beam.transform.localRotation * Vector2.up), _hits, 1000000);
            for ( var i = 0; i < hitsCount; i++ ) {
                var hit = _hits[i];
                if ( hit.collider && !hit.collider.isTrigger && (hit.collider != _ownerCollider) ) {
                    Beam.DealDamage(_hits[i].collider, Weapon.Damage);
                }
            }
        }

        void OnDrawGizmos() {
            Gizmos.DrawRay(Beam.transform.position,
                Beam.transform.TransformDirection(Beam.transform.localRotation * Vector2.up) * 10000);
        }
    }
}