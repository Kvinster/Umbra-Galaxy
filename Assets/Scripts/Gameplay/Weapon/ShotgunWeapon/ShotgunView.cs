using System.Collections.Generic;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Gameplay.Weapon.GunWeapon;

namespace STP.Gameplay.Weapon.ShotgunWeapon {
    public class ShotgunView : BaseWeaponView {
        public List<GunView> GunViews;

        public override void Init(CoreStarter starter, BaseShip ownerShip, BaseWeapon ownerWeapon) {
            foreach ( var view in GunViews ) {
                 view.Init(starter, ownerShip, ownerWeapon);
            }
        }
    }
}