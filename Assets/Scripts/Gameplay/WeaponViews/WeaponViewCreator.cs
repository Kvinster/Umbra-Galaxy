using UnityEngine;

using System.Collections.Generic;

using STP.Gameplay.WeaponGroup.Weapons;

namespace STP.Gameplay.WeaponViews {
    public class WeaponViewCreator {
        const string PrefabsPathFormat = "Prefabs/WeaponViews/";
        readonly CoreStarter _starter;

        readonly Dictionary<string, GameObject> _weaponViewsPrefabs = new Dictionary<string, GameObject>();
        
        public WeaponViewCreator(CoreStarter starter) {
            var bullets = Resources.LoadAll<GameObject>(PrefabsPathFormat);
            foreach ( var bullet in bullets ) {
                _weaponViewsPrefabs.Add(bullet.name, bullet);
            }
            _starter = starter;
        }

        public void RemoveWeaponView(Transform weaponMountPlace) {
            if ( weaponMountPlace.childCount == 0 ) {
                return;
            }
            Object.Destroy(weaponMountPlace.GetChild(0));
        }
        
        public void AddWeaponView(BaseShip ship, BaseWeapon weapon) {
            var weaponName = weapon.Name;
            if ( !_weaponViewsPrefabs.ContainsKey(weaponName) ) {
                Debug.LogError(string.Format("Can't find weapon view {0}", weaponName));
                return;
            }
            var weaponMountPlace = ship.WeaponMountPoint;
            if ( weaponMountPlace.childCount > 0 ) {
                Debug.Log("has attached weapon view. Remove it");
                RemoveWeaponView(weaponMountPlace);
            }
            
            var weaponView = Object.Instantiate(_weaponViewsPrefabs[weaponName], weaponMountPlace);
            var viewComp   = weaponView.GetComponent<BaseWeaponView>();
            viewComp.Init(_starter, ship, weapon);
        }
    }
}