using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Utils;

namespace STP.Gameplay.Weapon.LanceWeapon {
    public class LanceWeaponView : BaseWeaponView<Lance> {
        public BaseBeam   Beam;
        public GameObject ChargingImage;

        readonly Timer _timer = new Timer();

        void OnDestroy() {
            Weapon.StateChanged -= OnWeaponStateChanged;
        }

        void Update() {
            if ( _timer.Tick(Time.deltaTime) ) {
                Beam.gameObject.SetActive(false);
                _timer.Stop();
            }
            if ( Weapon.CurState == WeaponState.Fire ) {
                Beam.SetLength(Lance.MaxDistance);
            }
        }

        protected override void Init(CoreStarter starter, BaseShip ownerShip) {
            Weapon.StateChanged += OnWeaponStateChanged;
            OnWeaponStateChanged(Weapon.CurState);
        }

        void OnWeaponStateChanged(WeaponState newWeaponState) {
            ChargingImage.SetActive((newWeaponState == WeaponState.Charge) || (newWeaponState == WeaponState.Charged));
            if ( newWeaponState == WeaponState.Fire ) {
                Beam.gameObject.SetActive(true);
                _timer.Start(0.2f);
            }
        }
    }
}