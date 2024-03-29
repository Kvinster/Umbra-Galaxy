﻿using System;
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
	public class SpawnerBossController : BaseBoss {
		public BehaviourTree Tree;

		public float CollisionDamage = 25f;

		public float DeathEffectTime = 2f;

		public SpawnParams   SpawnParams;

		[NotNull] public Rigidbody2D           BossRigidbody;
		[NotNull] public BossMovementSubsystem MovementSubsystem;
		[Space]
		[NotNull] public AudioSource ShockwaveAudioSource;
		[NotNull] public AudioClip          ShockwaveStartSound;
		[NotNull] public LevelExplosionZone Shockwave;


		public List<BossGun> Guns;
		public List<Spawner> Spawners;

		SpawnerBossGunsSubsystem  _gunsSubsystem;
		SpawnerBossSpawnSubsystem _spawnSubsystem;

		LevelGoalManager _levelGoalManager;
		LevelManager     _levelManager;

		CameraShake _cameraShake;


		public override bool HighPriorityInit => true;

		public override HpSystem HpSystemComponent => HpSystem;

		public event Action OnBeginShooting;
		public event Action OnFinishShooting;

		protected void Update() {
			if ( Tree?.IsEmpty ?? true ) {
				return;
			}
			Tree.Tick();
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

			MovementSubsystem.Init(BossRigidbody, starter.Player.transform, HpSystem, starter.AreaRect);
			MovementSubsystem.SetActive(false);

			_gunsSubsystem = new SpawnerBossGunsSubsystem();
			_gunsSubsystem.Init(Guns, starter);

			_spawnSubsystem = new SpawnerBossSpawnSubsystem();
			var list = new List<ISpawner>(Spawners);
			_spawnSubsystem.Init(list, starter, SpawnParams);

			ShockwaveAudioSource.ignoreListenerPause = true;
		}

		public override void TakeDamage(float damage) {
			var wasAlive = HpSystem.IsAlive;
			HpSystem.TakeDamage(damage);
			if ( !HpSystem.IsAlive && wasAlive ) {
				Die();
			}
		}

		public override void Die(bool fromPlayer = true) {
			base.Die(fromPlayer);
			Tree = null;
			_gunsSubsystem.Deinit();
			PlayDeathVFXs().Forget();
		}

		protected override void Activate() {
			MovementSubsystem.SetActive(true);
			Tree = new BehaviourTree(
				new SequenceTask(
					new WaitTask(3f),
					new RepeatForeverTask(
						new SequenceTask(
							new WaitTask(1f),
							new CustomActionTask(() => OnBeginShooting?.Invoke()),
							new AlwaysSuccessDecorator(_gunsSubsystem.FireTask),
							new CustomActionTask(() => OnFinishShooting?.Invoke()),
							new WaitTask(2f),
							new AlwaysSuccessDecorator(MovementSubsystem.DashTask),
							new WaitTask(1f),
							new AlwaysSuccessDecorator(_spawnSubsystem.SpawnTask)
						)
					)
				)
			);
		}

		void OnCollisionEnter2D(Collision2D other) {
			other.TryTakeDamage(CollisionDamage);
		}

		async UniTask PlayDeathVFXs() {

			_cameraShake.Shake(DeathEffectTime, 1f).Forget();
			await UniTask.Delay(TimeSpan.FromSeconds(DeathEffectTime));

			// Run last shockwave
			Shockwave.gameObject.SetActive(true);
			Shockwave.transform.parent = transform.parent;
			ShockwaveAudioSource.PlayOneShot(ShockwaveStartSound);
			_cameraShake.Shake(1f, 4f).Forget();
			Destroy(gameObject);

			// win level
			_levelGoalManager.Advance();
			_levelManager.StartLevelWin();
		}
	}
}