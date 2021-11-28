using System;
using UnityEngine;

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using STP.Behaviour.Core.Enemy.BossSpawner;
using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;
using STP.Utils.GameComponentAttributes;
using UnityEngine.VFX;

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

		[NotNull] public LevelExplosionZone DeathShockwave;

		public float DeathEffectTime = 2f;

		[Header("public for editor only")]
		public BehaviourTree Tree;

		public VisualEffect ChargingEffect;
		public VisualEffect ChargingBulletEffect;

		SpawnerBossSpawnSubsystem _spawnSubsystem;
		SecondBossGunsSubsystem   _gunController;

		LevelGoalManager _levelGoalManager;
		LevelManager     _levelManager;

		CameraShake _cameraShake;

		public HpSystem HpSystemComponent => HpSystem;

		public static SecondBossController Instance { get; private set; }

		protected void Update() {
			Tree?.Tick();
		}

		protected override void Awake() {
			base.Awake();
			if ( Instance ) {
				Debug.LogErrorFormat(this, "{0}.{1}: more than one {2} instance is not supported",
					nameof(SecondBossController), nameof(Awake), nameof(SecondBossController));
				return;
			}
			Instance = this;
		}


		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);

			_levelGoalManager = starter.LevelGoalManager;
			_levelManager     = starter.LevelManager;
			_cameraShake      = starter.CameraShake;

			_spawnSubsystem = new SpawnerBossSpawnSubsystem();
			var list = new List<ISpawner>(Spawners);
			_spawnSubsystem.Init(list, starter, SpawnParams);

			_gunController = new SecondBossGunsSubsystem();
			_gunController.Init(Gun, ChargingEffect, ChargingBulletEffect, starter);

			MovementSubsystem.Init(OwnRigidbody, starter.Player.transform, HpSystemComponent);

			Tree = new BehaviourTree(
				new SequenceTask(
					new WaitTask(3f),
					new RepeatForeverTask(
						new SequenceTask(
							new AlwaysSuccessDecorator(_gunController.FireTask),
							new WaitTask(1f),
							new ParallelTask(
								_spawnSubsystem.SpawnTask,
								MovementSubsystem.DashTask
							)
						)
					)
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
			// win level
			if ( Tree != null ) {
				Tree = null;
				_gunController.Deinit();
				PlayDeathVfxs().Forget();
			}
		}

		async UniTask PlayDeathVfxs() {
			_cameraShake.Shake(DeathEffectTime, 1f).Forget();
			await UniTask.Delay(TimeSpan.FromSeconds(DeathEffectTime));
			DeathEffectRunner.StopVfx();

			// Run last shockwave
			DeathShockwave.gameObject.SetActive(true);
			DeathShockwave.transform.parent = transform.parent;
			_cameraShake.Shake(1f, 4f).Forget();
			Destroy(gameObject);

			await UniTask.Delay(TimeSpan.FromSeconds(3f));
			// win level
			_levelGoalManager.Advance();
			_levelManager.StartLevelWin();
		}
	}
}