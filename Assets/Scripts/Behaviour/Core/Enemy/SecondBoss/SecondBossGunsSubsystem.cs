using STP.Behaviour.Starter;
using STP.Utils.BehaviourTree.Tasks;

namespace STP.Behaviour.Core.Enemy.SecondBoss {
	public class SecondBossGunsSubsystem {
		SecondBossGun _gun;

		public BaseTask FireTask => 
			new SequenceTask(
				new RepeatUntilSuccess(
					new SequenceTask(
						new CustomActionTask(() => _gun.DefaultShootingSystem.DeltaTick()),
						new ConditionTask(() => _gun.DefaultShootingSystem.CanShoot))
					),
				new CustomActionTask("fire", () => _gun.DefaultShootingSystem.TryShoot())
			);
		

		public void Init(SecondBossGun gun, CoreStarter starter) {
			_gun = gun;
			_gun.Init(starter);
		}
	}
}