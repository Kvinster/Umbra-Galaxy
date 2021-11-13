using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class SpawnerBossGunsSubsystem {
		const float   FireTime   = 1f;
		const float   ChargeTime = 1f;
		
		BossGun _gun;

		public BaseTask FireTask => 
			new SequenceTask(
				new ParallelTask(
					new WaitTask(ChargeTime)
				),
				new CustomActionTask("Start fire", () => _gun.Laser.TryShoot()),
				new AlwaysSuccessDecorator(
					new ParallelTask(
						new SequenceTask(
							new WaitTask(FireTime),
							new ConditionTask(() => false)
						),
						new RepeatForeverTask(new CustomActionTask(() => _gun.Laser.Update()))
					)
				),
				new CustomActionTask("Stop fire", () => _gun.Laser.TryStopShoot()),
				new ParallelTask(
					new WaitTask(ChargeTime)
				)
			);
		

		public void Init(List<BossGun> guns, CoreStarter starter) {
			foreach ( var gun in guns ) {
				gun.Init(starter);
			}
			_gun = guns[0];
		}

		public void Deinit() {
			_gun.Laser.TryStopShoot();
		}
	}
}