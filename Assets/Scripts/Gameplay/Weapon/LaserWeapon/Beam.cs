using UnityEngine;

namespace STP.Gameplay.Weapon.LaserWeapon {
    public class Beam : MonoBehaviour{
        public SpriteRenderer BeamRenderer;
        
        const int Dps = 5;
        
        public void SetLength(float length) {
            BeamRenderer.size = new Vector2(BeamRenderer.size.x, length);
        }
        
        public void DealDamage(Collider2D other) {
            var damage = Dps * Time.deltaTime;
            var ship = other.GetComponent<IDestructable>();
            ship?.GetDamage(damage);
        }
    }
}