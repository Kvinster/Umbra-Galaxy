using UnityEngine;

using STP.Gameplay.Weapon.Common;
using STP.Gameplay.Weapon.LanceWeapon;
using STP.Utils;
using System;
using System.Collections.Generic;

namespace STP.Gameplay.Weapon.ImpulseWeapon {
   
    public class ImpulseWeaponView : BaseWeaponView {
        public SpriteRenderer ImpulseSprite;

        readonly Timer _disapperaTimer = new Timer();
        
        HashSet<Collider2D> _objects = new HashSet<Collider2D>();

        Impulse     _weapon;
        Collider2D  _ownerCollider;

        bool _isDisappearing;
        
        public override void Init(CoreStarter starter, BaseShip ownerShip, BaseWeapon ownerWeapon) {
            if ( !(ownerWeapon is Impulse laserWeapon) ) {
                return;
            }
            _weapon               = laserWeapon;
            _ownerCollider        = ownerShip.GetComponent<Collider2D>();
            _weapon.StateChanged += OnWeaponStateChanged;
            OnWeaponStateChanged(_weapon.CurState);
        }

        void OnWeaponStateChanged(WeaponState newWeaponState) {
            if ( newWeaponState == WeaponState.Fire ) {
                _disapperaTimer.Start(0.2f);
                _isDisappearing = true;
                _objects.RemoveWhere((x)=>!x);
                print("TEST. Count = " + _objects.Count);
                var copy = new HashSet<Collider2D>(_objects);
                foreach ( var obj in copy ) {
                    DealDamage(obj, _weapon.Damage);
                }
                _objects.Clear();
            }
        }

        void OnDestroy() {
            _weapon.StateChanged -= OnWeaponStateChanged;
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
            if ( _disapperaTimer.Tick(Time.deltaTime) ) {
                _isDisappearing = false;
                _disapperaTimer.Stop();
            }
            if ( _isDisappearing ) {
                SetImpulseAlpha(_disapperaTimer.NormalizedProgress);
            }
            else { 
                SetImpulseAlpha(_weapon.ChargeProgress);
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