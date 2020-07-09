using UnityEngine;

using STP.Gameplay.WeaponGroup.Weapons;
using STP.State;

namespace STP.Gameplay.WeaponGroup.Controls.PlayerControls {
   
    public class BulletManualControl : BaseWeaponControl<Gun> {
        BulletCreator _creator;
        PlayerShip    _ship;
        Rigidbody2D   _playerShipRigidbody;

        public BulletManualControl(Gun weapon, BulletCreator creator) : base(weapon) {
            _creator = creator;
        }

        public override void UpdateControl(float timePassed) {
            base.UpdateControl(timePassed);
            if ( Input.GetButton("Fire1") ) {
                Debug.Log("bullet try fire logging");
                Weapon.TryShoot();
                if ( Weapon.CurState == WeaponState.FIRE ) {
                    // _creator.CreateBullet(Bullets.PlayerBullet,  , , Weapon.BulletSpeed);
                }
            }
        }
    }
}