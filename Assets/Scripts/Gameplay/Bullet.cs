using UnityEngine;

namespace STP.Gameplay {
    public class Bullet : MonoBehaviour{
        public void OnCollisionEnter2D(Collision2D other) {
            Destroy(gameObject);
            var destructableComp = other.gameObject.GetComponent<Asteroid>();
            if ( destructableComp != null ) {
                Destroy(other.gameObject);
            }
        }
    }
}