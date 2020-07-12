using UnityEngine;

using System.Collections.Generic;

namespace STP.Gameplay.Weapon.Common {
    public class BulletCreator {
        const string PrefabsPathFormat     = "Prefabs/Bullets/";
        const string BeamPrefabsPathFormat = "Prefabs/Beams/";
        
        Dictionary<string, GameObject> _bulletPrefabs = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> _beamPrefabs   = new Dictionary<string, GameObject>();
        Transform                      _root;
        
        public BulletCreator(CoreStarter starter) {
            var bullets = Resources.LoadAll<GameObject>(PrefabsPathFormat);
            foreach ( var bullet in bullets ) {
                _bulletPrefabs.Add(bullet.name, bullet);
            }
            var beams = Resources.LoadAll<GameObject>(BeamPrefabsPathFormat);
            foreach ( var beam in beams ) {
                _beamPrefabs.Add(beam.name, beam);
            }
            _root = starter.BulletSpawnStock;
        }
        
        public GameObject CreateBullet(string bulletName, Vector2 startPosition, Vector2 flyDirection, float speed) {
            if ( !_bulletPrefabs.ContainsKey(bulletName) ) {
                Debug.LogError(string.Format("Can't find bullet {0} in loaded bullets", bulletName));
                return null;
            }
            var bullet                = Object.Instantiate(_bulletPrefabs[bulletName], _root);
            bullet.transform.position = startPosition;
            var rigidbody             = bullet.GetComponent<Rigidbody2D>();
            rigidbody.velocity        = flyDirection.normalized * speed;
            return bullet;
        }
    }
}