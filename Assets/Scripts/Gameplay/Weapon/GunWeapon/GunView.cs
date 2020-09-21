using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class GunView : BaseWeaponView<Gun> {
        void OnWeaponStateChanged(WeaponState newWeaponState) {
        }

        void OnDestroy() {
            Weapon.StateChanged -= OnWeaponStateChanged;
        }

        protected override void Init(CoreStarter starter, BaseShip ownerShip) {
            Weapon.StateChanged += OnWeaponStateChanged;
        }
    }
}
