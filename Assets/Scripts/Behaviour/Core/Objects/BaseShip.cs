using UnityEngine;

using System;

using STP.Gameplay;
using STP.Gameplay.Weapon.Common;
using STP.State.Core;
using STP.Utils;

namespace STP.Behaviour.Core.Objects {
    public abstract class BaseShip : BaseCoreComponent, IDestructable, ISideAccessable {
        public Transform WeaponMountPoint;
        public HpBar     HpBar;

        protected CoreShipState  ShipState;
        protected IWeaponControl WeaponControl;

        protected ShipInfo ShipInfo { get; private set; }

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
            HpBar.UpdateBar(ShipState.Hp / ShipInfo.Hp);
        }

        protected void InitShipInfo(ShipInfo shipInfo, CoreShipState shipState = null) {
            ShipInfo = shipInfo;
            ShipState = shipState ?? new CoreShipState(ShipInfo.Hp);

            HpBar.Init();
            HpBar.UpdateBar(ShipState.Hp / ShipInfo.Hp);
        }


        protected void UpdateWeaponControlState() {
            WeaponControl?.UpdateControl(Time.deltaTime);
        }

        protected abstract void OnShipDestroy();
    }
}
