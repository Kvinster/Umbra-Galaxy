using UnityEngine;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class Bullet : MonoBehaviour{
        GameObject _source;
        public virtual void Init(GameObject sourceShip) {
            _source = sourceShip;
        }
        
        public void OnCollisionEnter2D(Collision2D other) {
            if ( other.collider.gameObject == _source ) {
                return;
            }
            var destructableComp = other.gameObject.GetComponent<IDestructable>();
            destructableComp?.GetDamage();
            Destroy(gameObject);
        }
    }
}