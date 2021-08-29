﻿using STP.Behaviour.Core.Enemy.Boss;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy.BossSpawner {
	public class BossGun : DestructiblePart {
		[NotNull] public BossGunController GunController;

		public void Init(CoreStarter starter) {
			InitInternal();
			GunController.Init(starter);
		}
	}
}