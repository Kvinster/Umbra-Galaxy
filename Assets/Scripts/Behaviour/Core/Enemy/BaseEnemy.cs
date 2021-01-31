using UnityEngine;

using STP.Behaviour.Starter;
using STP.Behaviour.Utils;
using STP.Events;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public abstract class BaseEnemy : BaseCoreComponent {
		[NotNullOrEmpty] public string Name;
		[Space]
		public DelayedDestroyer      DeathSoundDestroyer;
		public Transform             DeathSoundPlayerTransform;
		public BaseSimpleSoundPlayer DeathSoundPlayer;

		Transform _tempObjRoot;

		protected override void InitInternal(CoreStarter starter) {
			_tempObjRoot = starter.TempObjectsRoot;
		}

		protected virtual void Die() {
			TryStartPlayingDeathSound();
			EventManager.Fire(new EnemyDestroyed(Name));
		}

		protected void TryStartPlayingDeathSound() {
			if ( !DeathSoundDestroyer || !DeathSoundPlayerTransform || !DeathSoundPlayer ) {
				return;
			}
			DeathSoundPlayerTransform.SetParent(_tempObjRoot);
			DeathSoundPlayerTransform.position = transform.position;
			DeathSoundDestroyer.StartDestroy();
			DeathSoundPlayer.Play();
		}
	}
}