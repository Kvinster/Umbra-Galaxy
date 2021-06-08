using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.Starter;
using STP.Core.ShootingsSystems;

namespace STP.Behaviour.Core.Enemy.Boss {
	public sealed class BossGunController : BaseCoreComponent {
		[Header("Parameters")]
		public float                ChargeTime;
		public ShootingSystemParams ShootingSystemParams;

		DefaultShootingSystem _defaultShootingSystem;

		bool  _isCharging;
		float _chargeTimer;

		public bool IsCharged { get; private set; }

		protected override void InitInternal(CoreStarter starter) {
			_defaultShootingSystem = new DefaultShootingSystem(starter.SpawnHelper, ShootingSystemParams);

			_chargeTimer = ChargeTime;
		}

		void Update() {
			if ( _isCharging ) {
				Assert.IsFalse(IsCharged);
				_chargeTimer = Mathf.Max(_chargeTimer - Time.deltaTime, 0f);
				if ( Mathf.Approximately(_chargeTimer, 0f) ) {
					_chargeTimer = ChargeTime;
					_isCharging  = false;
					IsCharged    = true;
				}
			}
		}

		public void StartCharging() {
			Assert.IsFalse(_isCharging);
			Assert.IsFalse(IsCharged);

			_isCharging = true;
		}

		public void Shoot() {
			Assert.IsFalse(_isCharging);
			Assert.IsTrue(IsCharged);

			if ( !_defaultShootingSystem.TryShoot() ) {
				Debug.LogErrorFormat("{0}.{1}: can't shoot for some reason", nameof(BossGunController), nameof(Shoot));
			}

			IsCharged = false;
		}
	}
}
