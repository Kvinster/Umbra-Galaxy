using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Utils;

namespace STP.Gameplay.Weapon.ImpulseWeapon {

    public class ImpulseWeaponView : BaseWeaponView<Impulse> {
        public SpriteRenderer ImpulseSprite;

        readonly Timer _disappearTimer = new Timer();

        readonly HashSet<Collider2D> _objects = new HashSet<Collider2D>();

        Collider2D  _ownerCollider; // TODO: unused?

        bool _isDisappearing;

        protected override void Init(CoreStarter starter, BaseShip ownerShip) {
            _ownerCollider       = ownerShip.GetComponent<Collider2D>();
            Weapon.StateChanged += OnWeaponStateChanged;
            OnWeaponStateChanged(Weapon.CurState);
        }

        void OnWeaponStateChanged(WeaponState newWeaponState) {
            if ( newWeaponState == WeaponState.Fire ) {
                _disappearTimer.Start(0.2f);
                _isDisappearing = true;
                _objects.RemoveWhere((x)=>!x);
                print("TEST. Count = " + _objects.Count);
                var copy = new HashSet<Collider2D>(_objects);
                foreach ( var obj in copy ) {
                    DealDamage(obj, Weapon.Damage);
                }
                _objects.Clear();
            }
        }

        void OnDestroy() {
            Weapon.StateChanged -= OnWeaponStateChanged;
        }

        void OnTriggerEnter2D(Collider2D other) {
            print("added " + other.gameObject.name);
            if ( !_objects.Contains(other) ) {
                _objects.Add(other);
            }
        }

        void OnTriggerExit2D(Collider2D other) {
            print("removed " + other.gameObject.name);
            _objects.Remove(other);
        }

        void Update() {
            if ( _disappearTimer.Tick(Time.deltaTime) ) {
                _isDisappearing = false;
                _disappearTimer.Stop();
            }
            if ( _isDisappearing ) {
                SetImpulseAlpha(_disappearTimer.NormalizedProgress);
            }
            else {
                SetImpulseAlpha(Weapon.ChargeProgress);
            }
        }

        void DealDamage(Collider2D other, float damage) {
            var ship = other.GetComponent<IDestructable>();
            ship?.GetDamage(damage);
        }

        void SetImpulseAlpha(float alpha) {
            var alphaNormalized = Mathf.Clamp01(alpha);
            var color           = ImpulseSprite.color;
            ImpulseSprite.color = new Color(color.r, color.g, color.b, alphaNormalized);
        }
    }
}