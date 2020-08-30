using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;

using Object = UnityEngine.Object;

namespace STP.Gameplay.Weapon.Common {
    public class WeaponViewCreator {
        const string PrefabsPathFormat = "Prefabs/Core/WeaponViews/";
        readonly CoreStarter _starter;

        readonly Dictionary<WeaponType, GameObject> _weaponViewsPrefabs = new Dictionary<WeaponType, GameObject>();

        public WeaponViewCreator(CoreStarter starter) {
            var bullets = Resources.LoadAll<GameObject>(PrefabsPathFormat);
            foreach ( var bullet in bullets ) {
                _weaponViewsPrefabs.Add((WeaponType)Enum.Parse(typeof(WeaponType), bullet.name, true), bullet);
            }
            _starter = starter;
        }

        public void RemoveWeaponView(Transform weaponMountPlace) {
            if ( weaponMountPlace.childCount == 0 ) {
                return;
            }
            Object.Destroy(weaponMountPlace.GetChild(0).gameObject);
        }

        public void AddWeaponView(BaseShip ship, BaseWeapon weapon) {
            if ( weapon == null ) {
                Debug.LogError("weapon object is null. Can't init view for it.");
                return;
            }
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