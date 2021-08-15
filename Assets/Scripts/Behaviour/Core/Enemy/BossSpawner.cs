using STP.Behaviour.Core.Enemy.Boss;
using STP.Behaviour.Starter;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;
using STP.Utils.GameComponentAttributes;


namespace STP.Behaviour.Core.Enemy {
	public class BossSpawner : BaseCoreComponent, IDestructible {
		int _fireCount;
		int _spawnCount;
		
		[NotNull] public BossGunController GunController;

		BehaviourTree _tree;

		bool HasGun => true;

		bool HasSpawners => true;

		void Update() {
			_tree.Tick();
		}

		protected override void InitInternal(CoreStarter starter) {
			_tree = new BehaviourTree(
				new SelectorTask(
					new SequenceTask(
						new ConditionTask(() => (HasGun && (_fireCount == _spawnCount)) || !HasSpawners),
						// Charge
						new AlwaysSuccessDecorator(
							new SequenceTask(
								new ConditionTask(() => !GunController.IsCharged),
								new CustomActionTask(() => {
									GunController.StartCharging();
								}),
								new RepeatUntilSuccess(
									new ConditionTask(() => GunController.IsCharged)
								)
							)
						),
						new CustomActionTask(() => {
							GunController.Shoot();
							_fireCount++;
						})
					),
					new SequenceTask(
						new ConditionTask(() => (HasSpawners && (_spawnCount < _fireCount)) || !HasGun),
						new WaitTask(3f),
						new CustomActionTask(() => {
							_spawnCount++;
						})
					)
				)
			);
		}

		public void TakeDamage(float damage) {
			
		}

		void StartSpawn() {
			
		}

		void StopSpawn() {
			
		}
	}
}