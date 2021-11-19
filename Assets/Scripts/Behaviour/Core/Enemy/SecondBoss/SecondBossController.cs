﻿using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Enemy.BossSpawner;
using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy.SecondBoss {
	public class SecondBossController : BaseEnemy, IHpSource, IDestructible {
		[Header("movement")]
		[NotNull] public BossMovementSubsystem MovementSubsystem;
		[Header("spawners")]
		[NotNull]        public SpawnParams   SpawnParams;
		[NotNullOrEmpty] public List<MineSpawner> Spawners;
		[Header("guns")]
		[NotNull] public SecondBossGun Gun;

		[NotNull] public Rigidbody2D OwnRigidbody;

		[NotNull] public LevelWinExplosionZone DeathShockwave;

		[Header("public for editor only")]
		public BehaviourTree Tree;

		SpawnerBossSpawnSubsystem _spawnSubsystem;
		SecondBossGunsSubsystem   _gunController;

		LevelGoalManager _levelGoalManager;

		public HpSystem HpSystemComponent => HpSystem;

		protected void Update() {
			Tree?.Tick();
		}

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);

			_levelGoalManager = starter.LevelGoalManager;

			_spawnSubsystem = new SpawnerBossSpawnSubsystem();
			var list = new List<ISpawner>(Spawners);
			_spawnSubsystem.Init(list, starter, SpawnParams);

			_gunController = new SecondBossGunsSubsystem();
			_gunController.Init(Gun, starter);

			MovementSubsystem.Init(OwnRigidbody, starter.Player.transform, HpSystemComponent);

			Tree = new BehaviourTree(
				new SequenceTask(
					new AlwaysSuccessDecorator(_gunController.FireTask),
					new WaitTask(1f),
					new AlwaysSuccessDecorator(MovementSubsystem.DashTask),
					new WaitTask(1f),
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

		public override void Die(bool fromPlayer = true) {
			base.Die(fromPlayer);
			_levelGoalManager.Advance();
		}
	}
}