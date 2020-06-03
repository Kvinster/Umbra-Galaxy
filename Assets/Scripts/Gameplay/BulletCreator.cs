using UnityEngine;

namespace STP.Gameplay {
    public class BulletCreator : MonoBehaviour {
        public GameObject BulletPrefab;
        public Transform  BulletRoot;

        public GameObject CreateBulletOn(Vector2 startPosition, Vector2 flyDirection, float speed) {
            var bullet = Instantiate(BulletPrefab, BulletRoot);
            bullet.transform.position = startPosition;
            var rigidbody = bullet.GetComponent<Rigidbody2D>();
            rigidbody.velocity = flyDirection.normalized * speed;
            return bullet;
        }
    }
}