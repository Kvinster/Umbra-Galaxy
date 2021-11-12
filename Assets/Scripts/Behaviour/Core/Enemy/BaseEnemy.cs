using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Events;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public abstract class BaseEnemy : BaseCoreComponent {
		[NotNullOrEmpty] public string Name;
		[Space]
		public SimpleDeathSoundPlayer DeathSoundPlayer;
		public VfxRunner              DeathEffectRunner;

		public bool DetachVfxOnDeath = true;
		
		public float StartHp;

		public event Action<BaseEnemy> OnDestroyed;

		protected HpSystem HpSystem;

		public bool IsAlive { get; protected set; }

		public abstract void OnBecomeVisibleForPlayer(Transform playerTransform);

		public abstract void OnBecomeInvisibleForPlayer();

		public abstract void SetTarget(Transform target);

		protected override void InitInternal(CoreStarter starter) {
			if ( DeathSoundPlayer ) {
				DeathSoundPlayer.Init(starter.TempObjectsRoot);
			}
			HpSystem = new HpSystem(StartHp);
			IsAlive = true;
		}


		public virtual void Die(bool fromPlayer = true) {
			if ( !IsAlive ) {
				return;
			}
			OnDestroyed?.Invoke(this);
			IsAlive = false;
			TryStartPlayingDeathSound();
			TryRunDeathEffect();
			if ( fromPlayer ) {
				EventManager.Fire(new EnemyDestroyed(Name));
			}
		}

		protected void TryStartPlayingDeathSound() {
			if ( DeathSoundPlayer ) {
				DeathSoundPlayer.Play();
			}
		}

		protected void TryRunDeathEffect() {
			if ( DeathEffectRunner ) {
				if ( DetachVfxOnDeath ) {
					DeathEffectRunner.transform.parent = transform.parent;
				}
				DeathEffectRunner.RunVfx(true);
			}
		}
	}
}