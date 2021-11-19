﻿using STP.Behaviour.Starter;
using STP.Gameplay.Weapon.LaserWeapon;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class BossGun : GameComponent {
		public float LaserDamage;
		
		[NotNull] public Transform       MountPoint;
		[NotNull] public Collider2D      OwnCollider;
		[NotNull] public LaserWeaponView LaserView;
		 
		public Laser Laser;

		public void Init(CoreStarter starter) {
			Laser = new Laser(MountPoint, OwnCollider, LaserDamage);
			LaserView.Init(Laser);
		}
	}
}