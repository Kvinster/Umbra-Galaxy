using UnityEngine;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public sealed class SpawnerBossWeaponSoundsPlayer : BaseCoreComponent {
		[NotNull] public SpawnerBossController BossController;
		[NotNull] public AudioSource           WeaponShootAudioSource;
		[NotNull] public BaseSimpleSoundPlayer WeaponStartShootSoundPlayer;

		protected override void OnDisable() {
			base.OnDisable();
			if ( BossController ) {
				BossController.OnBeginShooting  -= OnBeginShooting;
				BossController.OnFinishShooting -= OnFinishShooting;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			BossController.OnBeginShooting += OnBeginShooting;
			BossController.OnFinishShooting += OnFinishShooting;
		}

		void OnBeginShooting() {
			WeaponStartShootSoundPlayer.Play();
			WeaponShootAudioSource.Play();
		}

		void OnFinishShooting() {
			WeaponShootAudioSource.Stop();
		}
	}
}
