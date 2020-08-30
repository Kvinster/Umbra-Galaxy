using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Gameplay;
using STP.Gameplay.Weapon.Common;

namespace STP.Behaviour.Core.Objects {
    public abstract class BaseEnemyShip : BaseShip {
        CoreItemCreator _materialCreator;

        public List<Transform> DropItemsOnDeath;

        public override ConflictSide CurrentSide => ConflictSide.Aliens;

        public abstract bool CanShoot { get; }

        protected override void InitInternal(CoreStarter starter) {
            _materialCreator = starter.CoreItemCreator;
            WeaponControl    = starter.WeaponCreator.GetAIWeaponController(WeaponType.Gun, this);
            starter.WeaponViewCreator.AddWeaponView(this, WeaponControl.GetControlledWeapon());
            foreach ( var dropItem in DropItemsOnDeath ) {
                dropItem.gameObject.SetActive(false);
            }
        }

        protected override void OnShipDestroy() {
            _materialCreator.CreateRandomMaterial(transform.position);
            foreach ( var dropItem in DropItemsOnDeath ) {
                _materialCreator.SetParentForItem(dropItem);
                dropItem.rotation = Quaternion.identity;
                dropItem.gameObject.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
}
