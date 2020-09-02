using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Utils;

namespace STP.Gameplay.Weapon.Common {
    public abstract class BaseWeaponView : GameComponent {
        public abstract void Init(CoreStarter starter, BaseShip ownerShip, BaseWeapon ownerWeapon);
    }

    public abstract class BaseWeaponView<T> : BaseWeaponView where T : BaseWeapon {
        protected T Weapon;

        public sealed override void Init(CoreStarter starter, BaseShip ownerShip, BaseWeapon ownerWeapon) {
            if ( ownerWeapon is T weapon ) {
                Weapon = weapon;

                Init(starter, ownerShip);
            } else {
                Debug.LogErrorFormat("Invalid weapon type '{0}', expected '{1}'", ownerWeapon.GetType().Name,
                    typeof(T).Name);
            }
        }

        protected abstract void Init(CoreStarter starter, BaseShip ownerShip);
    }
}