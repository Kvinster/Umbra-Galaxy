using UnityEngine;

namespace STP.Gameplay {
    public class Bullet : MonoBehaviour{
        public void OnCollisionEnter2D(Collision2D other) {
            var desctrucableComp = other.gameObject.GetComponent<IDestructable>();
            if ( desctrucableComp != null ) {
                desctrucableComp.GetDamage();
            }
            Destroy(gameObject);
        }
    }
}