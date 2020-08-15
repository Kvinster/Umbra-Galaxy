using STP.Behaviour.Core;
using UnityEngine;

using System;

using STP.Gameplay.Weapon.Common;
using STP.State.Core;
using STP.Utils;
using STP.View;

namespace STP.Gameplay {
    public abstract class BaseShip : CoreComponent, IDestructable, ISideAccessable {
        public Transform WeaponMountPoint;
        public HpBar     HpBar;

        protected Rigidbody2D    Rigidbody2D;
        protected ShipState      ShipState;
        protected IWeaponControl WeaponControl;

        ShipInfo       _shipInfo;
        
        public event Action<BaseShip> OnShipDestroyed;
        
        public abstract ConflictSide CurrentSide { get; }

        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, WeaponMountPoint);

        public void GetDamage(float damageAmount = 1) {
            ShipState.Hp -= damageAmount;
            if ( ShipState.Hp <= 0 ) {
                OnShipDestroyed?.Invoke(this);
                OnShipDestroy();
            }
            HpBar.UpdateBar(ShipState.Hp / _shipInfo.Hp);
        }

        protected void InitShipInfo(ShipInfo shipInfo) {
            Rigidbody2D    = GetComponent<Rigidbody2D>();
            _shipInfo      = shipInfo;
            ShipState      = new ShipState(_shipInfo.Hp);
            
            HpBar.Init();
            HpBar.UpdateBar(1f);
        }

        protected void Move(Vector2 direction) { 
            var offsetVector = _shipInfo.MaxSpeed * direction;
            MoveUtils.ApplyMovingVector(Rigidbody2D, offsetVector);
        }
        
        protected void Rotate(Vector2 viewDirection) {
            MoveUtils.ApplyViewVector(transform, viewDirection);
        }
        

        protected void UpdateWeaponControlState() {
            WeaponControl?.UpdateControl(Time.deltaTime);
        }

        protected abstract void OnShipDestroy();
    }
}