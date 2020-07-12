using UnityEngine;

namespace STP.Gameplay.Weapon.GunWeapon {
    public class Bullet : MonoBehaviour{
        public void OnCollisionEnter2D(Collision2D other) {
            var destructableComp = other.gameObject.GetComponent<IDestructable>();
            destructableComp?.GetDamage();
            Destroy(gameObject);
        }
    }
}