using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.GunWeapon;
using STP.Gameplay.Weapon.MissileWeapon;
using STP.State;

namespace STP.Gameplay.Weapon.Common {
    public class BulletCreator {
        const string PrefabsPathFormat = "Prefabs/Core/Bullets/";
        
        Dictionary<string, GameObject> _bulletPrefabs = new Dictionary<string, GameObject>();
        Transform                      _root;
        
        AllianceManager                _allianceManager;
        
        public BulletCreator(Transform bulletRoot, AllianceManager allianceManager) {
            _allianceManager = allianceManager;
            var bullets = Resources.LoadAll<GameObject>(PrefabsPathFormat);
            foreach ( var bullet in bullets ) {
                _bulletPrefabs.Add(bullet.name, bullet);
            }
            _root = bulletRoot;
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
            InitBullet(bulletName, bullet, source.gameObject);
            return bullet;
        }

        void InitBullet(string bulletName, GameObject bullet, GameObject source) {
            if ( bulletName == Bullets.Missile ) {
                var bulletComp = bullet.GetComponent<Missile>();
                bulletComp.Init(source, _allianceManager);
            }
            else {
                var bulletComp = bullet.GetComponent<Bullet>();
                bulletComp.Init(source);
            }
        }
    }
    
    
}