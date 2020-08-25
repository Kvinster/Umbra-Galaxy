using UnityEngine;

using System;

using STP.Gameplay;
using STP.Gameplay.Weapon.Common;
using STP.State.Core;
using STP.Utils;

namespace STP.Behaviour.Core.Objects {
    public abstract class BaseShip : CoreComponent, IDestructable, ISideAccessable {
        public Transform WeaponMountPoint;
        public HpBar     HpBar;

        protected Rigidbody2D    Rigidbody2D;
        protected CoreShipState  ShipState;
        protected IWeaponControl WeaponControl;

        ShipInfo       _shipInfo;

        public event Action<BaseShip> OnShipDestroyed;

        public abstract ConflictSide CurrentSide { get; }

        public float CurHp => ShipState.Hp;

        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this, WeaponMountPoint);

        public void GetDamage(float damageAmount = 1) {
            ShipState.Hp -= damageAmount;
            if ( ShipState.Hp <= 0 ) {
                OnShipDestroyed?.Invoke(this);
                OnShipDestroy();
            }
            HpBar.UpdateBar(ShipState.Hp / _shipInfo.Hp);
        }

        protected void InitShipInfo(ShipInfo shipInfo, CoreShipState shipState = null) {
            Rigidbody2D    = GetComponent<Rigidbody2D>();
            
            _shipInfo = shipInfo;
            ShipState = shipState ?? new CoreShipState(_shipInfo.Hp);
            
            HpBar.Init();
            HpBar.UpdateBar(ShipState.Hp / _shipInfo.Hp);
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
