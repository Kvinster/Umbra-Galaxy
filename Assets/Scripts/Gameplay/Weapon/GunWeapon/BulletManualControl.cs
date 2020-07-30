using UnityEngine;

using STP.Gameplay.Weapon.Common;

namespace STP.Gameplay.Weapon.GunWeapon {
   
    public class BulletManualControl : BaseWeaponControl<Gun> {
        Rigidbody2D   _playerShipRigidbody;

        public BulletManualControl(Gun weapon) : base(weapon) { }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( Input.GetButton("Fire1") ) {
                Debug.Log("bullet try fire logging");
                Weapon.TryShoot();
            }
        }
    }
}