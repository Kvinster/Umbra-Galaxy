using System.Collections.Generic;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Gameplay.Weapon.GunWeapon;

namespace STP.Gameplay.Weapon.ShotgunWeapon {
    public class ShotgunView : BaseWeaponView<Shotgun> {
        public List<GunView> GunViews;

        protected override void Init(CoreStarter starter, BaseShip ownerShip) {
            foreach ( var view in GunViews ) {
                 view.Init(starter, ownerShip, Weapon);
            }
        }
    }
}