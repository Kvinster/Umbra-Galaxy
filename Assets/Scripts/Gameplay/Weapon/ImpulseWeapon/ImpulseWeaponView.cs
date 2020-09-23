using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Utils;

namespace STP.Gameplay.Weapon.ImpulseWeapon {

    public class ImpulseWeaponView : BaseWeaponView<Impulse> {
        public SpriteRenderer ImpulseSprite;

        readonly Timer _disappearTimer = new Timer();

        bool _isDisappearing;

        protected override void Init(CoreStarter starter, BaseShip ownerShip) {
            Weapon.StateChanged += OnWeaponStateChanged;
            OnWeaponStateChanged(Weapon.CurState);
        }

        void OnWeaponStateChanged(WeaponState newWeaponState) {
            if ( newWeaponState == WeaponState.Fire ) {
                _disappearTimer.Start(0.2f);
                _isDisappearing = true;
            }
        }

        void OnDestroy() {
            Weapon.StateChanged -= OnWeaponStateChanged;
        }

        void Update() {
            if ( _disappearTimer.Tick(Time.deltaTime) ) {
                _isDisappearing = false;
                _disappearTimer.Stop();
            }
            SetImpulseAlpha(_isDisappearing ? _disappearTimer.NormalizedProgress : Weapon.ChargeProgress);
        }

        void SetImpulseAlpha(float alpha) {
            var alphaNormalized = Mathf.Clamp01(alpha);
            var color           = ImpulseSprite.color;
            ImpulseSprite.color = new Color(color.r, color.g, color.b, alphaNormalized);
        }
    }
}