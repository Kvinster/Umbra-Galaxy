using STP.Behaviour.Starter;
using STP.Core.ShootingsSystems;
using STP.Utils;

namespace STP.Behaviour.Core.Enemy.SecondBoss {
	public class SecondBossGun : GameComponent {
		public ShootingSystemParams ShootingSystemParams;
		
		
		public DefaultShootingSystem DefaultShootingSystem;


		public void Init(CoreStarter starter) {
			DefaultShootingSystem = new DefaultShootingSystem(starter.SpawnHelper, ShootingSystemParams);
		}
	}
}