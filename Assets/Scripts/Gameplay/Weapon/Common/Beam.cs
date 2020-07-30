using UnityEngine;

namespace STP.Gameplay.Weapon.Common {
    public class Beam : MonoBehaviour{
        public SpriteRenderer BeamRenderer;
        
        const int Dps = 5;
        
        public void SetLength(float length) {
            BeamRenderer.size = new Vector2(BeamRenderer.size.x, length);
        }
        
        public void DealDamage(Collider2D other) {
            DealDamage(other, Dps * Time.deltaTime);
        }
        
        public void DealDamage(Collider2D other, float damage) {
            var ship = other.GetComponent<IDestructable>();
            ship?.GetDamage(damage);
        }
    }
}