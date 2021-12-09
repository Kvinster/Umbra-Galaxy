using UnityEngine;

using System.Collections.Generic;

using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

namespace STP.Behaviour.Core.UI {
	public sealed class PowerUpTimerUisManager : GameComponent {
		[Header("Parameters")]
		public float DropInterval = 0.1f;
		public float VerticalMoveAnimDuration = 0.5f;
		[Header("Dependencies")]
		[NotNullOrEmpty] public List<PowerUpTimerUi> PowerUpTimerUis = new List<PowerUpTimerUi>();
		[NotNullOrEmpty] public List<Transform> Positions = new List<Transform>();

		readonly List<PowerUpTimerUi> _activeTimers = new List<PowerUpTimerUi>();

		readonly HashSet<Tween> _activeAnims = new HashSet<Tween>();

		public void Init(PlayerManager playerManager) {
			foreach ( var powerUpTimerUi in PowerUpTimerUis ) {
				powerUpTimerUi.Init(playerManager);
				powerUpTimerUi.OnBecameActive   += OnTimerBecameActive;
				powerUpTimerUi.OnBecameInactive += OnTimerBecameInactive;
			}
		}

		public void Deinit() {
			foreach ( var powerUpTimerUi in PowerUpTimerUis ) {
				if ( powerUpTimerUi ) {
					powerUpTimerUi.OnBecameActive   -= OnTimerBecameActive;
					powerUpTimerUi.OnBecameInactive -= OnTimerBecameInactive;
				}
			}
			ResetActiveAnims();
		}

		void OnTimerBecameActive(PowerUpTimerUi powerUpTimerUi) {
			var position = GetPosition(_activeTimers.Count);
			if ( !position ) {
				return;
			}
			powerUpTimerUi.transform.localPosition = position.localPosition;
			_activeTimers.Add(powerUpTimerUi);
		}

		void OnTimerBecameInactive(PowerUpTimerUi powerUpTimerUi) {
			var index = _activeTimers.IndexOf(powerUpTimerUi);
			if ( index < 0 ) {
				Debug.LogError("Unexpected scenario");
				return;
			}
			_activeTimers.RemoveAt(index);
			ResetActiveAnims();
			for ( var i = index; i < _activeTimers.Count; ++i ) {
				var timer    = _activeTimers[i];
				var position = Positions[i];
				var anim = DOTween.Sequence().AppendInterval(DropInterval * (i - index))
					.Append(timer.transform.DOLocalMove(position.localPosition, VerticalMoveAnimDuration)
						.SetEase(Ease.Linear));
				anim.OnComplete(() => _activeAnims.Remove(anim));
				_activeAnims.Add(anim);
			}
		}

		void ResetActiveAnims() {
			foreach ( var activeAnim in _activeAnims ) {
				activeAnim?.Kill();
			}
			_activeAnims.Clear();
		}

		Transform GetPosition(int index) {
			if ( index >= Positions.Count ) {
				Debug.LogError("Not enough Positions");
				return null;
			}
			return Positions[index];
		}
	}
}
