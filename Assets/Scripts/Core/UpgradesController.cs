using UnityEngine.Assertions;

using System;

using STP.Common;
using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class UpgradesController : BaseStateController {
		public const int MaxPlayerCharacteristicLevel = 5;

		readonly UpgradesControllerState _state;
		readonly XpController            _xpController;

		readonly PlayerCharacteristicsConfig _characteristicsConfig;

		public int UpgradePoints {
			get => _state.UpgradePoints;
			private set {
				if ( UpgradePoints == value ) {
					return;
				}
				_state.UpgradePoints = value;
				OnUpgradePointsChanged?.Invoke(UpgradePoints);
			}
		}

		public event Action<PlayerCharacteristicType, int> OnCharacteristicLevelChanged;
		public event Action<int>                           OnUpgradePointsChanged;

		public UpgradesController(GameState gameState, XpController xpController) {
			_state        = gameState.UpgradesControllerState;
			_xpController = xpController;

			_characteristicsConfig = PlayerCharacteristicsConfig.LoadConfig();

			_xpController.OnLevelUp += OnLevelUp;
		}

		public bool TryUpgrade(PlayerCharacteristicType characteristicType) {
			Assert.AreNotEqual(characteristicType, PlayerCharacteristicType.Unknown);
			if ( CanUpgrade(characteristicType) ) {
				var curLevel = GetCurCharacteristicLevel(characteristicType);
				UpgradePoints -= curLevel + 1;
				SetCharacteristicLevel(characteristicType, curLevel + 1);
				return true;
			}
			return false;
		}

		public bool CanUpgrade(PlayerCharacteristicType characteristicType) {
			var curLevel = GetCurCharacteristicLevel(characteristicType);
			return (curLevel < MaxPlayerCharacteristicLevel) && (UpgradePoints >= curLevel + 1);
		}

		public int GetCurCharacteristicLevel(PlayerCharacteristicType characteristicType) {
			Assert.AreNotEqual(characteristicType, PlayerCharacteristicType.Unknown);
			return _state.GetCharacteristicLevel(characteristicType);
		}

		public float GetCurConfigFireRate() {
			return _characteristicsConfig.GetFireRate(GetCurCharacteristicLevel(PlayerCharacteristicType.FireRate));
		}

		public float GetCurConfigDamage() {
			return _characteristicsConfig.GetDamage(GetCurCharacteristicLevel(PlayerCharacteristicType.Damage));
		}

		public float GetCurConfigMaxHp() {
			return _characteristicsConfig.GetHp(GetCurCharacteristicLevel(PlayerCharacteristicType.MaxHp));
		}

		public float GetCurConfigMovementSpeed() {
			return _characteristicsConfig.GetMovementSpeed(
				GetCurCharacteristicLevel(PlayerCharacteristicType.MovementSpeed));
		}

		public void CheatAddUpgradePoints(int upgradePoints) {
			UpgradePoints += upgradePoints;
		}

		void OnLevelUp() {
			++UpgradePoints;
		}

		void SetCharacteristicLevel(PlayerCharacteristicType characteristicType, int level) {
			Assert.AreNotEqual(characteristicType, PlayerCharacteristicType.Unknown);
			_state.SetCharacteristicLevel(characteristicType, level);
			OnCharacteristicLevelChanged?.Invoke(characteristicType, level);
		}
	}
}
