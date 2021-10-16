using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Core;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class SpawnerBossController : BaseCoreComponent, IHpSource, IDestructible {

		public BehaviourTree Tree;

		public float CollisionDamage = 25f;
		public float StartHp         = 10000;
		
		public SpawnParams   SpawnParams;

		[NotNull] public Rigidbody2D                  BossRigidbody;
		[NotNull] public SpawnerBossMovementSubsystem MovementSubsystem;
		
		public List<BossGun> Guns;
		public List<Spawner> Spawners;

		SpawnerBossGunsSubsystem     _gunsSubsystem;
		SpawnerBossSpawnSubsystem    _spawnSubsystem;

		HpSystem _hpSystem;
		
		public static SpawnerBossController Instance { get; private set; }

		public HpSystem HpSystem => _hpSystem;
		public override bool HighPriorityInit => true;
		
		
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
			Tree.Tick();
		}
		
		void OnDestroy() {
			_gunsSubsystem?.Deinit();
			_spawnSubsystem?.Deinit();
		}

		protected override void InitInternal(CoreStarter starter) {
			_hpSystem = new HpSystem(StartHp);

			_hpSystem.OnDied += () => Destroy(gameObject);
			
			MovementSubsystem.Init(BossRigidbody, starter.Player.transform);
			
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
					new AlwaysSuccessDecorator(_spawnSubsystem.BehaviourTree),
					new SequenceTask(
						new ConditionTask("Is everything destroyed", () => !_gunsSubsystem.HasGuns && !_spawnSubsystem.HasSpawners),
						new CustomActionTask("destroy boss object", () => Destroy(gameObject))
					)
				)
			);
		}

		public void TakeDamage(float damage) {
			_hpSystem.TakeDamage(damage);
		}
		
		void OnCollisionEnter2D(Collision2D other) {
			var destructible = other.gameObject.GetComponent<IDestructible>();
			destructible?.TakeDamage(CollisionDamage);
		}
	}
}