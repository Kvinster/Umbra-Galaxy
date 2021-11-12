using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using STP.Behaviour.Starter;
using STP.Core;
using STP.Manager;
using STP.Utils;
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
		[NotNull] public SpawnerBossMovementSubsystem MovementSubsystem;

		[NotNull] public LevelWinExplosionZone Shockwave;

		public List<BossGun> Guns;
		public List<Spawner> Spawners;

		SpawnerBossGunsSubsystem     _gunsSubsystem;
		SpawnerBossSpawnSubsystem    _spawnSubsystem;

		LevelManager _levelManager;

		CameraShake _cameraShake;
		
		public static SpawnerBossController Instance { get; private set; }

		public override bool HighPriorityInit => true;
		
		public HpSystem HpSystemComponent => HpSystem;

		public override void OnBecomeVisibleForPlayer(Transform playerTransform) {
			// Do nothing
		}

		public override void OnBecomeInvisibleForPlayer() {
			// Do nothing
		}

		public override void SetTarget(Transform target) {
			// Do nothing
		}

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
			_spawnSubsystem?.Deinit();
		}

		protected override void InitInternal(CoreStarter starter) {
			base.InitInternal(starter);
			Shockwave.gameObject.SetActive(false);
			_levelManager = starter.LevelManager;
			_cameraShake  = starter.CameraShake;
			
			MovementSubsystem.Init(BossRigidbody, starter.Player.transform, HpSystem);

			_gunsSubsystem = new SpawnerBossGunsSubsystem();
			_gunsSubsystem.Init(Guns, starter, MovementSubsystem);

			_spawnSubsystem = new SpawnerBossSpawnSubsystem();
			_spawnSubsystem.Init(Spawners, starter, SpawnParams, MovementSubsystem);

			Tree = new BehaviourTree(
				new SequenceTask(
					new AlwaysSuccessDecorator(_gunsSubsystem.BehaviourTree),
					new AlwaysSuccessDecorator(
						new SequenceTask(
							new CustomActionTask("set dash speed", () => MovementSubsystem.Dash()),
							new WaitTask(2f),
							new CustomActionTask("stop dash", () => MovementSubsystem.EndDash())
						)
					),
					new AlwaysSuccessDecorator(_spawnSubsystem.BehaviourTree)
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
			_gunsSubsystem.Deinit();
			_spawnSubsystem.Deinit();
			_cameraShake.Shake(DeathEffectTime, 1f).Forget();
			AsyncUtils.DelayedAction(SomeAction, DeathEffectTime);
		}

		void OnCollisionEnter2D(Collision2D other) {
			other.TryTakeDamage(CollisionDamage);
		}

		void SomeAction() {
			DeathEffectRunner.StopVfx();
			Shockwave.gameObject.SetActive(true);
			Shockwave.transform.parent = transform.parent;
			_cameraShake.Shake(1f, 4f).Forget();
			Destroy(gameObject);
			AsyncUtils.DelayedAction(_levelManager.StartLevelWin, 3f);
		}
	}
}