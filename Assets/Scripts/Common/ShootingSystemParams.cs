﻿using UnityEngine;

using System;

using STP.Utils.GameComponentAttributes;

namespace STP.Common {
	[Serializable]
	public sealed class ShootingSystemParams {
		[NotNull] 
		public GameObject   BulletPrefab;
		[NotNull]
		public Transform    BulletOrigin;
		public Transform    RotationSource;
		public float        BulletSpeed;
		public float        ReloadTime;
		public float        BulletDamage;
		public Collider2D[] IgnoreColliders;

		public ShootingSystemParams ShallowCopy() {
			return MemberwiseClone() as ShootingSystemParams;
		}
	}
}