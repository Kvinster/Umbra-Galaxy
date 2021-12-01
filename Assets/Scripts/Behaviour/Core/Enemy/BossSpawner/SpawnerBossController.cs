using System;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class SpawnerBossController : BaseEnemy, IHpSource, IDestructible {
		public BehaviourTree Tree;

		public float CollisionDamage = 25f;

		public float DeathEffectTime = 2f;

		public SpawnParams   SpawnParams;

		[NotNull] public Rigidbody2D                  BossRigidbody;
		[NotNull] public BossMovementSubsystem MovementSubsystem;

		[NotNull] public LevelExplosionZone Shockwave;

		public List<BossGun> Guns;
		public List<Spawner> Spawners;

		SpawnerBossGunsSubsystem  _gunsSubsystem;
		SpawnerBossSpawnSubsystem _spawnSubsystem;

		LevelGoalManager _levelGoalManager;
		LevelManager     _levelManager;

		CameraShake _cameraShake;

		public static SpawnerBossController Instance { get; private set; }

		public override bool HighPriorityInit => true;

		public HpSystem HpSystemComponent => HpSystem;


		protected override void Awake() {
			base.Awake();
			if ( Instance ) {
				Debug.LogErrorFormat(this, "{0}.{1}: more than one {2} instance is not supported",
					nameof(SpawnerBossController), nameof(Awake), nameof(SpawnerBossController));
				return;
			}
			Instance = this;
		}

		protected void Update() {
			Tree?.Tick();
		}

		void OnDestroy() {
			_gunsSubsystem?.Deinit();
		}

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			Shockwave.gameObject.SetActive(false);
			_levelGoalManager = starter.LevelGoalManager;
			_levelManager     = starter.LevelManager;
			_cameraShake      = starter.CameraShake;

			MovementSubsystem.Init(BossRigidbody, starter.Player.transform, HpSystem);

			_gunsSubsystem = new SpawnerBossGunsSubsystem();
			_gunsSubsystem.Init(Guns, starter);

			_spawnSubsystem = new SpawnerBossSpawnSubsystem();
			var list = new List<ISpawner>(Spawners);
			_spawnSubsystem.Init(list, starter, SpawnParams);

			Tree = new BehaviourTree(
				new SequenceTask(
					new WaitTask(3f),
					new RepeatForeverTask(
						new SequenceTask(
							new AlwaysSuccessDecorator(_gunsSubsystem.FireTask),
							new WaitTask(1f),
							new AlwaysSuccessDecorator(MovementSubsystem.DashTask),
							new WaitTask(1f),
							new AlwaysSuccessDecorator(_spawnSubsystem.SpawnTask)
						)
					)
				)
			);
		}

		public void TakeDamage(float damage) {
			HpSystem.TakeDamage(damage);
			if ( !HpSystem.IsAlive ) {
				Die();
			}
		}

		public override void Die(bool fromPlayer = true) {
			base.Die(fromPlayer);
			Tree = null;
			_levelGoalManager.Advance();
			_gunsSubsystem.Deinit();
			PlayDeathVFXs().Forget();
		}

		void OnCollisionEnter2D(Collision2D other) {
			other.TryTakeDamage(CollisionDamage);
		}

		async UniTask PlayDeathVFXs() {
			_cameraShake.Shake(DeathEffectTime, 1f).Forget();
			await UniTask.Delay(TimeSpan.FromSeconds(DeathEffectTime));
			DeathEffectRunner.StopVfx();

			// Run last shockwave
			Shockwave.gameObject.SetActive(true);
			Shockwave.transform.parent = transform.parent;
			_cameraShake.Shake(1f, 4f).Forget();
			Destroy(gameObject);

			// win level
			_levelManager.StartLevelWin();
		}
	}
}