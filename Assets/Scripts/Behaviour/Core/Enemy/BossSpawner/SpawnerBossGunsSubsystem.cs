using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.LaserWeapon;
using STP.Utils;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class SpawnerBossGunsSubsystem {
		public float  FireTime   = 1f;
		public float  ChargeTime = 1f;
		List<BossGun> _guns;

		BossGun _selectedGun;

		public BaseTask BehaviourTree { get; private set; }

		public bool HasGuns => _guns.Count > 0;
		
		public void Init(List<BossGun> guns, CoreStarter starter, SpawnerBossMovementSubsystem movementSubsystem) {
			_guns = guns;
			foreach ( var gun in _guns ) {
				gun.Init(starter);
				gun.OnDiedEvent += OnGunDestroyed;
			}

			var bt = new SequenceTask(
				// Select available gun
				new ConditionTask("Is gun selected", TrySelectGun),
				new ParallelTask(
					new WaitTask(ChargeTime),
					new CustomActionTask("Start spinning", () => movementSubsystem.SetMovementType(MovementType.SpinUp))
				),
				new CustomActionTask("Start fire", () => _selectedGun.Laser.TryShoot()),
				new AlwaysSuccessDecorator(
					new ParallelTask(
						new SequenceTask(
							new WaitTask(FireTime),
							new ConditionTask(() => false)
						),
						new RepeatForeverTask(new CustomActionTask(() => _selectedGun.Laser.Update()))
					)
				),
				new CustomActionTask("Stop fire", () => _selectedGun.Laser.TryStopShoot()),
				new ParallelTask(
					new WaitTask(ChargeTime),
					new CustomActionTask("Stop spinning", () => movementSubsystem.SetMovementType(MovementType.SpinDown))
				)
			);
			BehaviourTree = bt;
		}

		public void Deinit() {
			foreach ( var gun in _guns ) {
				gun.OnDiedEvent -= OnGunDestroyed;
				gun.Laser.TryStopShoot();
			}
		}

		void OnGunDestroyed(DestructiblePart destroyedGun) {
			_guns.RemoveAll(x => x == destroyedGun);
			if ( _selectedGun == destroyedGun ) {
				BehaviourTree.InstantFinishTask(TaskStatus.Failure);
			}
		}
		
		bool TrySelectGun() {
			if ( _guns.Count == 0 ) {
				return false;
			}
			_selectedGun = RandomUtils.GetRandomElement(_guns);
			return true;
		}
	}
}