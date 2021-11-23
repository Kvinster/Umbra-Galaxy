using STP.Behaviour.Starter;
using STP.Utils.BehaviourTree.Tasks;
using UnityEngine.VFX;

namespace STP.Behaviour.Core.Enemy.SecondBoss {
	public class SecondBossGunsSubsystem {
		VisualEffect  _chargeEffect;
		SecondBossGun _gun;

		public BaseTask FireTask =>
			new SequenceTask(
				new CustomActionTask(() => _chargeEffect.Play()),
				new RepeatUntilSuccess(
					new SequenceTask(
						new CustomActionTask(() => _gun.DefaultShootingSystem.DeltaTick()),
						new ConditionTask(() => _gun.DefaultShootingSystem.CanShoot))
					),
				new CustomActionTask(() => _chargeEffect.Stop()),
				new RepeatUntilSuccess(
					new SequenceTask(
						new ConditionTask(() => _chargeEffect.aliveParticleCount == 0))
				),
				new CustomActionTask("fire", () => _gun.DefaultShootingSystem.TryShoot())
			);


		public void Init(SecondBossGun gun, VisualEffect chargeEffect, CoreStarter starter) {
			_gun          = gun;
			_chargeEffect = chargeEffect;
			_gun.Init(starter);
		}
	}
}