using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Utils.GameComponentAttributes;

namespace STP.Gameplay.Weapon.LaserWeapon {
    public sealed class LaserWeaponView : BaseWeaponView<Laser> {
        [NotNull] public BaseBeam Beam;

        void OnDestroy() {
            Weapon.StateChanged -= OnWeaponStateChanged;
        }

        void Update() {
            if ( Weapon.CurState == WeaponState.Fire ) {
                Beam.SetLength(Weapon.CurHitDistance);
            }
        }

        protected override void Init(CoreStarter starter, BaseShip ownerShip) {
            Weapon.StateChanged += OnWeaponStateChanged;
            OnWeaponStateChanged(Weapon.CurState);
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