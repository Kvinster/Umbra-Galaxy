using UnityEngine;

using STP.Gameplay;
using STP.Utils;

namespace STP.Behaviour.Core.Objects.Traps {
    public class StaticTrap : GameBehaviour {
        const float Damage = 5;
        void OnCollisionEnter2D(Collision2D other) {
            var destructableComp = other.gameObject.GetComponent<IDestructable>();
            destructableComp?.GetDamage(Damage);
        }
    }
}