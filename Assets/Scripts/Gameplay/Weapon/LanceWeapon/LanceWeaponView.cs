using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.Common;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Gameplay.Weapon.LanceWeapon {
    public class LanceWeaponView : BaseWeaponView<Lance> {
        const float MinChargeEmissionRate       = 1;
        const float MaxChargeEmissionRate       = 10;
        const float MinSecondaryChargeStartSize = 20f;
        const float MaxSecondaryChargeStartSize = 50f;
        const float ChargeForceFieldStrength    = 10f;
        const float FireForceFieldStrength      = -100f;

        [NotNull] public ParticleSystem           ChargeParticleSystem;
        [NotNull] public ParticleSystem           SecondaryChargeParticleSystem;
        [NotNull] public ParticleSystemForceField ChargeForceField;
        [NotNull] public VfxBeam                  Beam;

        readonly Timer _timer = new Timer();

        bool _fading;

        Transform _customSimulationSpaceTrans;
        Transform CustomSimulationSpaceTrans {
            get {
                if ( !_customSimulationSpaceTrans ) {
                    var go = new GameObject("[LanceWeaponView_CustomSimulationSpaceObj]");
                    _customSimulationSpaceTrans = go.transform;
                    _customSimulationSpaceTrans.SetParent(null);
                }
                return _customSimulationSpaceTrans;
            }
        }

        void OnDestroy() {
            Weapon.StateChanged -= OnWeaponStateChanged;
        }

        void Update() {
            if ( Weapon.CurState == WeaponState.Charge ) {
                SetChargeProgress(Weapon.ChargingProgress);
            }
            if ( _fading ) {
                if ( _timer.Tick(Time.deltaTime) ) {
                    Beam.gameObject.SetActive(false);
                    _timer.Stop();
                    _fading = false;
                } else {
                    Beam.Alpha = 1f - _timer.NormalizedProgress;
                }
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
            if ( newWeaponState == WeaponState.Fire ) {
                Beam.transform.SetParent(transform, false);
                Beam.gameObject.SetActive(true);

                _timer.Start(0.5f);
                _fading = true;

                var beamTrans = Beam.transform;
                beamTrans.SetParent(null, true);
                beamTrans.position = transform.position;
                beamTrans.rotation = transform.rotation;

                ChargeParticleSystem.Emit(6);
                ChargeForceField.gravity = new ParticleSystem.MinMaxCurve(FireForceFieldStrength);

                var chargeMain = ChargeParticleSystem.main;
                chargeMain.simulationSpace = ParticleSystemSimulationSpace.Custom;
                CustomSimulationSpaceTrans.position = ChargeParticleSystem.transform.position;
                CustomSimulationSpaceTrans.rotation = transform.rotation;
                chargeMain.customSimulationSpace = CustomSimulationSpaceTrans;

            }
            if ( newWeaponState == WeaponState.Charge ) {
                ChargeForceField.gravity = new ParticleSystem.MinMaxCurve(ChargeForceFieldStrength);
                ChargeParticleSystem.Clear();
                ChargeParticleSystem.Play(true);
                var chargeMain = ChargeParticleSystem.main;
                chargeMain.simulationSpace = ParticleSystemSimulationSpace.Local;

                SetChargeProgress(0f);
            } else if ( newWeaponState != WeaponState.Charged) {
                ChargeParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        void SetChargeProgress(float progress) {
            progress = Mathf.Sqrt(progress);
            var chargeEmission = ChargeParticleSystem.emission;
            chargeEmission.rateOverTime =
                new ParticleSystem.MinMaxCurve(Mathf.Lerp(MinChargeEmissionRate, MaxChargeEmissionRate, progress));
            var secondaryMain = SecondaryChargeParticleSystem.main;
            secondaryMain.startSize = new ParticleSystem.MinMaxCurve(Mathf.Lerp(MinSecondaryChargeStartSize,
                MaxSecondaryChargeStartSize, progress));
        }
    }
}