using UnityEngine;

using System.Collections.Generic;

using STP.Gameplay.Weapon.GunWeapon;

namespace STP.Gameplay.Weapon.Common {
    public class BulletCreator {
        const string PrefabsPathFormat = "Prefabs/Bullets/";
        
        Dictionary<string, GameObject> _bulletPrefabs = new Dictionary<string, GameObject>();
        Transform                      _root;
        
        public BulletCreator(CoreStarter starter) {
            var bullets = Resources.LoadAll<GameObject>(PrefabsPathFormat);
            foreach ( var bullet in bullets ) {
                _bulletPrefabs.Add(bullet.name, bullet);
            }
            _root = starter.BulletSpawnStock;
        }
        
        public GameObject CreateBullet(BaseShip source, string bulletName, Vector2 startPosition, Vector2 flyDirection, float speed) {
            if ( !_bulletPrefabs.ContainsKey(bulletName) ) {
                Debug.LogError(string.Format("Can't find bullet {0} in loaded bullets", bulletName));
                return null;
            }
            var bullet                = Object.Instantiate(_bulletPrefabs[bulletName], _root);
            bullet.transform.position = startPosition;
            var rigidbody             = bullet.GetComponent<Rigidbody2D>();
            rigidbody.velocity        = flyDirection.normalized * speed;
            var bulletComp            = bullet.GetComponent<Bullet>();
            bulletComp.Init(source.gameObject);
            return bullet;
        }
    }
}