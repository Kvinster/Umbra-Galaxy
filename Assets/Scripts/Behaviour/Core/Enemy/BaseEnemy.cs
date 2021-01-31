﻿using UnityEngine;

using STP.Behaviour.Starter;
using STP.Events;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public abstract class BaseEnemy : BaseCoreComponent {
		[NotNullOrEmpty] public string Name;
		[Space]
		public SimpleDeathSoundPlayer DeathSoundPlayer;

		protected bool IsAlive;

		protected override void InitInternal(CoreStarter starter) {
			if ( DeathSoundPlayer ) {
				DeathSoundPlayer.Init(starter.TempObjectsRoot);
			}

			IsAlive = true;
		}

		protected virtual void Die(bool fromPlayer = true) {
			if ( !IsAlive ) {
				return;
			}
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