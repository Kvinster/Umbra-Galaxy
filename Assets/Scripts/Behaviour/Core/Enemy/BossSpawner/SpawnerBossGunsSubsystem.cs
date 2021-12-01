using System.Collections.Generic;
using STP.Behaviour.Starter;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class SpawnerBossGunsSubsystem {
		const float   FireTime   = 1f;
		const float   ChargeTime = 1f;

		List<BossGun> _guns;

		public BaseTask FireTask =>
			new SequenceTask(
				new ParallelTask(
					new WaitTask(ChargeTime)
				),
				new CustomActionTask("Start fire", () => {
					foreach ( var gun in _guns ) {
						gun.Laser.TryShoot();
					}
				}),
				new AlwaysSuccessDecorator(
					new ParallelTask(
						new SequenceTask(
							new WaitTask(FireTime),
							new ConditionTask(() => false)
						),
						new RepeatForeverTask(new CustomActionTask(() => {
									foreach ( var gun in _guns ) {
										gun.Laser.Update();
									}
								}
							)
						)
					)
				),
				new CustomActionTask("Stop fire", () => {
					foreach ( var gun in _guns ) {
						gun.Laser.TryStopShoot();
					}
				}),
				new ParallelTask(
					new WaitTask(ChargeTime)
				)
			);


		public void Init(List<BossGun> guns, CoreStarter starter) {
			foreach ( var gun in guns ) {
				gun.Init(starter);
			}
			_guns = guns;
		}

		public void Deinit() {
			foreach ( var gun in _guns ) {
				gun.Laser.TryStopShoot();
			}
		}
	}
}