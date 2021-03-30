﻿using UnityEngine;

using System;

using STP.Behaviour.Core;
using STP.Config;

namespace STP.Core {
	public sealed class XpController : BaseStateController {
		
		public class BoxedValue<T> where T : IComparable<T> {
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
		
		readonly LevelController _levelController;

		XpConfig _xpConfig;
		
		public readonly BoxedValue<int> Xp            = new BoxedValue<int>();
		public readonly BoxedValue<int> Level         = new BoxedValue<int>();
		public readonly BoxedValue<int> LevelUpsCount = new BoxedValue<int>();

		public bool IsMaxLevelReached => Level == _xpConfig.LevelUpInfos.Count;
		public int  LevelXpCap => !IsMaxLevelReached ? _xpConfig.LevelUpInfos[Level].NeededXp : int.MaxValue;
		
		public XpController() {
			LoadConfig();
		}
		
		public void AddLevelXp(int value) {
			if ( value < 0 ) {
				Debug.LogWarning($"Strange xp amount {value}. Ignoring");
				return;
			}
			Xp.Value += value;
			if ( !IsMaxLevelReached && (Xp.Value > LevelXpCap) ) {
				Xp.Value -= LevelXpCap;
				Level.Value++;
				LevelUpsCount.Value++;
			}
		}

		public void UseLevelUp() {
			LevelUpsCount.Value--;
		}

		public PlayerLevelUpInfo GetLevelUpInfo(int level) {
			if ( (level < 0) || (level >= _xpConfig.LevelUpInfos.Count) ) {
				Debug.LogError($"can't get level up info. Invalid level {level}");
				return null;
			}
			return _xpConfig.LevelUpInfos[level];
		}
		
		public ShipVisual GetShipVisuals(ShipType type) {
			return _xpConfig.GetShipVisual(type);
		}
		
		public int GetDestroyedEnemyXp(string enemyName) {
			return _xpConfig.GetDestroyedEnemyXp(enemyName);
		}

		void LoadConfig() {
			_xpConfig = Resources.Load<XpConfig>("XpConfig");
		}
	}
}
