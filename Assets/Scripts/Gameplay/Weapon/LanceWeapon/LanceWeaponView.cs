using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Utils;

namespace STP.Gameplay.Weapon.LanceWeapon {
    public class LanceWeaponView : BaseWeaponView {
        public Beam       Beam;
        public GameObject ChargingImage;

        Timer       _timer = new Timer();

        Lance       _weapon;
        Collider2D  _ownerCollider;

        readonly RaycastHit2D[] _hits = new RaycastHit2D[10];

        public override void Init(CoreStarter starter, BaseShip ownerShip, BaseWeapon ownerWeapon) {
            if ( !(ownerWeapon is Lance laserWeapon) ) {
                return;
            }
            _weapon = laserWeapon;
            _ownerCollider = ownerShip.GetComponent<Collider2D>();
            _weapon.StateChanged += OnWeaponStateChanged;
            OnWeaponStateChanged(_weapon.CurState);
        }

        void OnWeaponStateChanged(WeaponState newWeaponState) {
            ChargingImage.SetActive(newWeaponState == WeaponState.Charge || newWeaponState == WeaponState.Charged);
            if ( newWeaponState == WeaponState.Fire ) {
                Beam.gameObject.SetActive(true);
                _timer.Start(0.2f);
            }
        }

        void OnDestroy() {
            _weapon.StateChanged -= OnWeaponStateChanged;
        }

        void Update() {
            if ( _timer.Tick(Time.deltaTime) ) {
                Beam.gameObject.SetActive(false);
                _timer.Stop();
            }
            if ( _weapon.CurState == WeaponState.Fire ) {
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
                    Beam.DealDamage(_hits[i].collider, _weapon.Damage);
                }
            }
        }

        void OnDrawGizmos() {
            Gizmos.DrawRay(Beam.transform.position,
                Beam.transform.TransformDirection(Beam.transform.localRotation * Vector2.up) * 10000);
        }
    }
}