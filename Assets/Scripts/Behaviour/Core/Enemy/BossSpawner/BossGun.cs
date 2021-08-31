using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.LaserWeapon;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class BossGun : DestructiblePart {
		public float LaserDamage;
		
		[NotNull] public Transform       MountPoint;
		[NotNull] public Collider2D      OwnCollider;
		[NotNull] public LaserWeaponView LaserView;
		 
		public Laser Laser;

		public void Init(CoreStarter starter) {
			InitInternal();
			Laser = new Laser(MountPoint, OwnCollider, LaserDamage);
			LaserView.Init(Laser);
		}
	}
}