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
	public sealed class RailgunBossController : BaseBoss {
		public float CollisionDamage = 25f;

		[Header("portal")]
		[NotNull] public Portal Portal;
		[NotNull] public Transform PortalAppearPosition;
		public           float     StartDelay;
		[Header("movement")]
		[NotNull] public BossMovementSubsystem MovementSubsystem;
		[Header("spawners")]
		[NotNull]        public SpawnParams   SpawnParams;
		[NotNullOrEmpty] public List<MineSpawner> Spawners;
		[Header("guns")]
		[NotNull] public BossRailgun Gun;

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

		public override HpSystem HpSystemComponent => HpSystem;

		public event Action OnBeginChargingGun;
		public event Action OnShootGun;

		void Update() {
			if ( Tree?.IsEmpty ?? true ) {
				return;
			}
			Tree.Tick();
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
			MovementSubsystem.SetActive(false);

			Portal.PlayTargetAppearAnim(transform, PortalAppearPosition, () => {
				MovementSubsystem.SetActive(true);
				Tree = new BehaviourTree(
					new SequenceTask(
						new WaitTask(3f),
						new RepeatForeverTask(
							new SequenceTask(
								new CustomActionTask(() => OnBeginChargingGun?.Invoke()),
								new AlwaysSuccessDecorator(_gunController.FireTask),
								new CustomActionTask(() => OnShootGun?.Invoke()),
								new WaitTask(1f),
								new ParallelTask(
									_spawnSubsystem.SpawnTask,
									MovementSubsystem.DashTask
								)
							)
						)
					)
				);
			}, StartDelay);

			DeathShockwave.gameObject.SetActive(false);
		}

		public override void TakeDamage(float damage) {
			HpSystem.TakeDamage(damage);
			if ( !HpSystem.IsAlive ) {
				Die();
			}
		}

		void OnCollisionEnter2D(Collision2D other) {
			other.TryTakeDamage(CollisionDamage);
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