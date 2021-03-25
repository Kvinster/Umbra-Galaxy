using UnityEngine;

using System;

using STP.Behaviour.Core;
using STP.Behaviour.Starter;
using STP.Events;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.EndlessLevel.Enemies {
	public abstract class BaseEnemy : BaseEndlessLevelComponent {
		[NotNullOrEmpty] public string Name;
		[Space]
		public SimpleDeathSoundPlayer DeathSoundPlayer;

		public event Action<BaseEnemy> OnDestroyed;
		
		protected bool IsAlive;
		
		protected override void InitInternal(EndlessLevelStarter starter) {
			if ( DeathSoundPlayer ) {
				DeathSoundPlayer.Init(starter.TmpObjectsRoot);
			}
			IsAlive = true;
		}

		protected virtual void Die(bool fromPlayer = true) {
			if ( !IsAlive ) {
				return;
			}
			OnDestroyed?.Invoke(this);
			IsAlive = false;
			TryStartPlayingDeathSound();
			if ( fromPlayer ) {
				EventManager.Fire(new EnemyDestroyed(Name));
			}
		}

		protected void TryStartPlayingDeathSound() {
			if ( DeathSoundPlayer ) {
				DeathSoundPlayer.Play();
			}
		}
	}
}