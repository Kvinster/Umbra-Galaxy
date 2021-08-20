using System.Collections.Generic;
using STP.Utils;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class BossSpawnerGunsSubsystem : GameComponent {
		public List<BossGun> Guns;

		BossGun _selectedGun;
		

		public bool HasGuns => Guns.Count > 0;

		public BaseTask GunSubBt { get; private set; }
		
		public int FireCount { get; private set; }
		
		
		public void Init() {
			foreach ( var gun in Guns ) {
				gun.OnDiedEvent += OnGunDestroyed;
			}
			
			GunSubBt = new SequenceTask( 
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
			if ( Guns.Count == 0 ) {
				return false;
			}
			_selectedGun = RandomUtils.GetRandomElement(Guns);
			return true;
		}

		void Fire() {
			_selectedGun.GunController.Shoot();
			FireCount++;
		}

		public void Deinit() {
			foreach ( var gun in Guns ) {
				gun.OnDiedEvent -= OnGunDestroyed;
			}
		}

		void OnGunDestroyed(DestructiblePart destroyedGun) {
			Guns.RemoveAll(x => x == destroyedGun);
			if ( _selectedGun == destroyedGun ) {
				GunSubBt.InstantFinishTask(TaskStatus.Failure);
			}
		}
	}
}