using UnityEngine;

using System;

namespace STP.Core {
	public class HpSystem {
		float _hp;

		public float MaxHp { get; private set; }

		public float Hp {
			get => _hp;

			private set {
				var oldHp = _hp;
				_hp = value;
				if ( !Mathf.Approximately(oldHp, value) ) {
					OnHpChanged?.Invoke(value);
				}
			}
		}

		public bool IsAlive => (Hp > 0f);

		public event Action<float> OnHpChanged;

		public event Action OnDied;

		public HpSystem(float startHp) {
			Hp    = startHp;
			MaxHp = startHp;
		}

		public void TakeDamage(float damage) {
			if ( !IsAlive ) {
				return;
			}
			Hp = Mathf.Max(Hp - damage, 0);
			if ( Mathf.Approximately(Hp, 0) ) {
				OnDied?.Invoke();
			}
		}

		public void AddHp(float hp) {
			if ( hp < 0f ) {
				Debug.LogErrorFormat("{0}.{1}: trying to add negative hp", nameof(HpSystem), nameof(AddHp));
				return;
			}
			Hp = Mathf.Min(Hp + hp, MaxHp);
		}

		public void ResetHp() {
			Hp = MaxHp;
		}
	}
}
