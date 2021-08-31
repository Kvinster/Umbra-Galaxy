using UnityEngine;

using STP.Gameplay.Weapon.Common;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Gameplay.Weapon.LaserWeapon {
    public sealed class LaserWeaponView : GameComponent {
        [NotNull] public VfxBeam Beam;

        Laser _laser;
        
        void OnDestroy() {
            _laser.StateChanged -= OnWeaponStateChanged;
        }

        void Update() {
            if ( _laser.CurState == WeaponState.Fire ) {
                Beam.SetLength(_laser.CurHitDistance);
            }
        }

        public void Init(Laser laser) {
            _laser             =  laser;
            laser.StateChanged += OnWeaponStateChanged;
            OnWeaponStateChanged(_laser.CurState);
        }

        void OnWeaponStateChanged(WeaponState newWeaponState) {
            Beam.gameObject.SetActive(newWeaponState == WeaponState.Fire);
        }

        void OnDrawGizmos() {
            Gizmos.DrawRay(Beam.transform.position,
                Beam.transform.TransformDirection(Beam.transform.rotation * Vector2.up) * 10000);
        }
    }
}