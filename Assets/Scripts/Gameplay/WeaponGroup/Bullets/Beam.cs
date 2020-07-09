using UnityEngine;

using STP.Gameplay.WeaponGroup.Weapons;

namespace STP.Gameplay {
    public class Beam : MonoBehaviour{
        const int dps = 1;
        
        BaseWeapon _weapon;

        public void Init(BaseWeapon weapon) {
            _weapon = weapon;
        }
        
        void Update() {
            if ( _weapon == null ) {
                return;
            }
            if ( _weapon.CurState == WeaponState.CHARGED ) {
                Destroy(gameObject);
            }
        }

        void OnTriggerStay2D(Collider2D other) {
            if ( _weapon == null ) {
                return;
            }
            var damage = dps * Time.deltaTime;
            var ship = other.GetComponent<IDestructable>();
            ship.GetDamage(damage);
        }

    }
}