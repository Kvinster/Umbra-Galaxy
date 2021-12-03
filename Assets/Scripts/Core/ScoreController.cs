using UnityEngine;

using System;

using STP.Config;

namespace STP.Core {
	public sealed class ScoreController : BaseStateController {
		public sealed class BoxedValue<T> where T : IComparable<T> {
			T _value;

			public T Value {
				get => _value;
				set {
					if ( _value.CompareTo(value) == 0 ) {
						return;
					}
					_value = value;
					OnValueChanged?.Invoke(_value);
				}
			}

			public static implicit operator T(BoxedValue<T> value)
			{
				return value.Value;
			}

			public event Action<T> OnValueChanged;
		}

		ScoreConfig _scoreConfig;

		public readonly BoxedValue<int> Score = new BoxedValue<int>();

		public ScoreController() {
			LoadConfig();
		}

		public void AddEnemyDestroyedXp(string enemyName) {
			var value = GetDestroyedEnemyXp(enemyName);
			if ( value <= 0 ) {
				Debug.LogWarning($"Strange xp amount {value}. Ignoring");
				return;
			}
			Score.Value += value;
		}

		int GetDestroyedEnemyXp(string enemyName) {
			return _scoreConfig.GetDestroyedEnemyScore(enemyName);
		}

		void LoadConfig() {
			_scoreConfig = Resources.Load<ScoreConfig>("ScoreConfig");
		}
	}
}
