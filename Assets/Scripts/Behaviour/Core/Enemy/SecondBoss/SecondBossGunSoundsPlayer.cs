using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy.SecondBoss {
	public sealed class SecondBossGunSoundsPlayer : BaseCoreComponent {
		[NotNull] public SecondBossController  BossController;
		[NotNull] public AudioSource           WeaponChargeAudioSource;
		[NotNull] public BaseSimpleSoundPlayer WeaponShootSoundPlayer;

		protected override void OnDisable() {
			base.OnDisable();
			if ( BossController ) {
				BossController.OnBeginChargingGun -= OnBeginCharge;
				BossController.OnShootGun          -= OnWeaponShoot;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			BossController.OnBeginChargingGun += OnBeginCharge;
			BossController.OnShootGun          += OnWeaponShoot;
		}

		void OnBeginCharge() {
			WeaponChargeAudioSource.Play();
		}

		void OnWeaponShoot() {
			WeaponChargeAudioSource.Stop();
			WeaponShootSoundPlayer.Play();
		}
	}
}
