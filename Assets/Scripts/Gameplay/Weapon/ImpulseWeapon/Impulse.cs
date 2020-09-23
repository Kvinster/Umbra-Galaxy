using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.Chargeable;
using STP.Gameplay.Weapon.Common;
using STP.Utils;

namespace STP.Gameplay.Weapon.ImpulseWeapon {
    public sealed class Impulse : ChargeableWeapon  {
        const float Radius = 80.75f;

        public override WeaponType Name   => WeaponType.Impulse;
        public override float      Damage => 5f;

        protected override float ChargingTime => 0.5f;

        readonly BaseShip        _owner;
        readonly TriggerNotifier _triggerNotifier;

        readonly HashSet<IDestructable> _targets  = new HashSet<IDestructable>();
        readonly HashSet<IDestructable> _toRemove = new HashSet<IDestructable>();

        bool _isFiring;

        public float ChargeProgress {
            get {
                switch ( CurState ) {
                    case WeaponState.Charged: {
                        return 1f;
                    }
                    case WeaponState.Charge: {
                        return Timer.NormalizedProgress;
                    }
                    default: {
                        return 0f;
                    }
                }
            }
        }

        public Impulse(BaseShip owner) {
            _owner = owner;

            var notifierGo = new GameObject("[ImpulseTriggerNotifier]");
            notifierGo.transform.SetParent(_owner.transform); // wanted to use WeaponMountPoint, but it's offset af
            notifierGo.transform.localPosition = Vector3.zero;
            notifierGo.transform.localRotation = Quaternion.identity;
            var trigger = notifierGo.AddComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius    = Radius;
            _triggerNotifier = notifierGo.AddComponent<TriggerNotifier>();
            _triggerNotifier.OnTriggerEnter += OnTriggerEnter; // TODO: unsubscribe or don't need to? :thinking:
            _triggerNotifier.OnTriggerExit  += OnTriggerExit;
        }

        public override void ReleaseCharging() {
            base.ReleaseCharging();

            if ( CurState == WeaponState.Fire ) {
                _isFiring = true;
                foreach ( var target in _targets ) {
                    if ( _toRemove.Contains(target) ) {
                        continue;
                    }
                    target.GetDamage(Damage);
                }
                _isFiring = false;
                if ( _toRemove.Count > 0) {
                    _targets.RemoveWhere(x => _toRemove.Contains(x));
                    _toRemove.Clear();
                }
            }
        }

        void OnTriggerEnter(GameObject other) {
            var destructible = other.GetComponentInChildren<IDestructable>();
            if ( destructible != null ) {
                if ( _isFiring && _toRemove.Contains(destructible) ) {
                    _toRemove.Remove(destructible);
                } else {
                    _targets.Add(destructible);
                }
            }
        }

        void OnTriggerExit(GameObject other) {
            var destructible = other.GetComponentInChildren<IDestructable>();
            if ( destructible != null ) {
                if ( _isFiring ) {
                    _toRemove.Add(destructible);
                } else {
                    _targets.Remove(destructible);
                }
            }
        }
    }
}