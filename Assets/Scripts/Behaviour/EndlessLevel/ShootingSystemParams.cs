﻿using UnityEngine;

using System;

using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.EndlessLevel {
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
	}
}