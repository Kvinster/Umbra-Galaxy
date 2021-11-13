using STP.Behaviour.Core.Enemy.BossSpawner;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy.SecondBoss {
	public class SecondBossController : BaseCoreComponent {
		[NotNull] public BossMovementSubsystem MovementSubsystem;
		
		protected override void InitInternal(CoreStarter starter) {
		}
	}
}