using UnityEngine;

using STP.Gameplay.WeaponGroup.Weapons;

namespace STP.Gameplay {
    public class Beam : MonoBehaviour{
        public SpriteRenderer    BeamRenderer;
        
        const int dps = 5;
        
        public void SetLength(float length) {
            BeamRenderer.size = new Vector2(BeamRenderer.size.x, length);
        }
        
        public void DealDamage(Collider2D other) {
            var damage = dps * Time.deltaTime;
            var ship = other.GetComponent<IDestructable>();
            ship?.GetDamage(damage);
        }
    }
}