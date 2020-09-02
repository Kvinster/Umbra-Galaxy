using UnityEngine;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class Bullet : BaseBullet {
        protected float Damage        = 5f;
        protected float MaxFlightTime = 15f;
        
        GameObject _source;
        
        public void Init(GameObject sourceShip) {
            InitTimer(MaxFlightTime);
            _source = sourceShip;
        }
        
        public void OnCollisionEnter2D(Collision2D other) {
            if ( other.collider.gameObject == _source ) {
                return;
            }
            var destructableComp = other.gameObject.GetComponent<IDestructable>();
            destructableComp?.GetDamage(Damage);
            Destroy(gameObject);
        }
    }
}