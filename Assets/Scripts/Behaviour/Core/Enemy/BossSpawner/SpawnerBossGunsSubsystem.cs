using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class SpawnerBossGunsSubsystem {
		List<BossGun> _guns;

		BossGun _selectedGun;

		public BaseTask BehaviourTree { get; private set; }

		public bool HasGuns => _guns.Count > 0;
		
		public void Init(List<BossGun> guns, CoreStarter starter) {
			_guns = guns;
			foreach ( var gun in _guns ) {
				gun.Init(starter);
				gun.OnDiedEvent += OnGunDestroyed;
			}
			
			BehaviourTree = new SequenceTask( 
				// Select available gun
				new ConditionTask(TrySelectGun),
				// Charge
				new AlwaysSuccessDecorator(
					new SequenceTask(
						new ConditionTask(TryStartCharging),
						new RepeatUntilSuccess(
							new ConditionTask(() => _selectedGun.GunController.IsCharged)
						)
					)
				),
				// Fire
				new CustomActionTask(_selectedGun.GunController.Shoot));
		}

		bool TryStartCharging() {
			if ( _selectedGun.GunController.IsCharged ) {
				return false;
			}
			_selectedGun.GunController.StartCharging();
			return true;
		}

		void Fire() {
			_selectedGun.GunController.Shoot();
		}

		public void Deinit() {
			foreach ( var gun in _guns ) {
				gun.OnDiedEvent -= OnGunDestroyed;
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