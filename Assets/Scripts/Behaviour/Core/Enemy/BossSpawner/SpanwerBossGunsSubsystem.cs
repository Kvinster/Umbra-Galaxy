using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class BossSpawnerGunsSubsystem {
		List<BossGun> _guns;

		BossGun _selectedGun;
		
		public bool HasGuns => _guns.Count > 0;

		public BaseTask GunSubBt { get; private set; }
		
		public int FireCount { get; private set; }
		
		public void Init(List<BossGun> guns, CoreStarter starter) {
			_guns = guns;
			foreach ( var gun in _guns ) {
				gun.Init(starter);
				gun.OnDiedEvent += OnGunDestroyed;
			}
			
			GunSubBt = new SequenceTask( 
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
				new CustomActionTask(Fire));
		}

		bool TryStartCharging() {
			if ( _selectedGun.GunController.IsCharged ) {
				return false;
			}
			_selectedGun.GunController.StartCharging();
			return true;
		}

		bool TrySelectGun() {
			if ( _guns.Count == 0 ) {
				return false;
			}
			_selectedGun = RandomUtils.GetRandomElement(_guns);
			return true;
		}

		void Fire() {
			_selectedGun.GunController.Shoot();
			FireCount++;
		}

		public void Deinit() {
			foreach ( var gun in _guns ) {
				gun.OnDiedEvent -= OnGunDestroyed;
			}
		}

		void OnGunDestroyed(DestructiblePart destroyedGun) {
			_guns.RemoveAll(x => x == destroyedGun);
			if ( _selectedGun == destroyedGun ) {
				GunSubBt.InstantFinishTask(TaskStatus.Failure);
			}
		}
	}
}