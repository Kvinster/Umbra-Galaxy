using UnityEngine;

using System.Collections.Generic;

namespace STP.Config {
	[CreateAssetMenu(fileName = "PlayerCharacteristicsConfig",
		menuName              = "ScriptableObjects/PlayerCharacteristicsConfig", order = 1)]
	public sealed class PlayerCharacteristicsConfig : ScriptableObject {
		public List<float> FireRates      = new List<float>();
		public List<float> Damages        = new List<float>();
		public List<int>   MaxHps         = new List<int>();
		public List<float> MovementSpeeds = new List<float>();

		public float GetFireRate(int fireRateLevel) {
			return GetValue(FireRates, fireRateLevel);
		}

		public float GetDamage(int damageLevel) {
			return GetValue(Damages, damageLevel);
		}

		public int GetHp(int hpLevel) {
			return GetValue(MaxHps, hpLevel);
		}

		public float GetMovementSpeed(int movementSpeedLevel) {
			return GetValue(MovementSpeeds, movementSpeedLevel);
		}

		T GetValue<T>(List<T> list, int level) {
			return list[Mathf.Clamp(level, 0, list.Count - 1)];
		}

		public static PlayerCharacteristicsConfig LoadConfig() {
			return Resources.Load<PlayerCharacteristicsConfig>("PlayerCharacteristicsConfig");
		}
	}
}
