using System.Collections.Generic;
using STP.Behaviour.Core.Enemy.BossSpawner;
using STP.Behaviour.Starter;
using STP.Core;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.Core.Enemy.SecondBoss {
	public class SecondBossController : BaseEnemy, IHpSource, IDestructible {
		[NotNull] public BossMovementSubsystem MovementSubsystem;

		[NotNull]        public SpawnParams   SpawnParams;
		[NotNullOrEmpty] public List<MineSpawner> Spawners; 
		
		[NotNull] public Rigidbody2D OwnRigidbody;

		[NotNull] public LevelWinExplosionZone DeathShockwave;
		
		[Header("public for editor only")]
		public BehaviourTree Tree;
		
		SpawnerBossSpawnSubsystem _spawnSubsystem;
		
		public HpSystem HpSystemComponent => HpSystem;
		
		protected void Update() {
			Tree?.Tick();
		}

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			
			_spawnSubsystem = new SpawnerBossSpawnSubsystem();
			var list = new List<ISpawner>(Spawners);
			_spawnSubsystem.Init(list, starter, SpawnParams);
			
			MovementSubsystem.Init(OwnRigidbody, starter.Player.transform, HpSystemComponent);
			
			Tree = new BehaviourTree(
				new ParallelTask(
					new AlwaysSuccessDecorator(MovementSubsystem.DashTask),
					new AlwaysSuccessDecorator(_spawnSubsystem.SpawnTask)
				)
			);
			
			DeathShockwave.gameObject.SetActive(false);
		}

		public void TakeDamage(float damage) {
			HpSystem.TakeDamage(damage);
			if ( !HpSystem.IsAlive ) {
				Die();
			}
		}
		
		

	}
}