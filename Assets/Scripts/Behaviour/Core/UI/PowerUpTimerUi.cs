using UnityEngine;
using UnityEngine.UI;

using System;

using STP.Common;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;
using TMPro;

namespace STP.Behaviour.Core.UI {
	public sealed class PowerUpTimerUi : GameComponent {
		[Header("Parameters")]
		public float AnimDuration;
		public PowerUpType PowerUpType;
		[Header("Dependencies")]
		[NotNull] public Image TimerFill;
		[NotNull] public TMP_Text TimerText;
		[Space]
		[NotNull] public GameObject Root;
		[NotNull] public Transform MoveTransform;
		[NotNull] public Transform ShowPosition;
		[NotNull] public Transform HidePosition;

		PlayerManager _playerManager;

		Sequence _anim;
		int      _lastTimer;

		public bool IsActive { get; private set; }

		public event Action<PowerUpTimerUi> OnBecameActive;
		public event Action<PowerUpTimerUi> OnBecameInactive;

		void OnDisable() {
			_anim?.Kill();
		}

		void Update() {
			if ( !IsActive ) {
				return;
			}
			var timeRaw = _playerManager.GetPowerUpCurTime(PowerUpType);
			var time    = Mathf.CeilToInt(timeRaw);
			TimerFill.fillAmount = Mathf.Clamp01(timeRaw / _playerManager.GetPowerUpTotalTime(PowerUpType));
			if ( _lastTimer != time ) {
				TimerText.text = time.ToString();
				_lastTimer     = time;
			}
		}

		public void Init(PlayerManager playerManager) {
			_playerManager                   =  playerManager;
			_playerManager.OnPowerUpStarted  += OnPowerUpStarted;
			_playerManager.OnPowerUpFinished += OnPowerUpFinished;

			if ( _playerManager.HasActivePowerUp(PowerUpType) ) {
				OnPowerUpStarted(PowerUpType);
			} else {
				SetHidden();
			}
		}

		void OnPowerUpStarted(PowerUpType powerUpType) {
			if ( powerUpType != PowerUpType ) {
				return;
			}
			IsActive = true;
			Root.SetActive(true);
			_anim?.Kill();
			_anim = DOTween.Sequence()
				.Append(GetAnimTo(ShowPosition))
				.OnComplete(() => _anim = null);
			OnBecameActive?.Invoke(this);
		}

		void OnPowerUpFinished(PowerUpType powerUpType) {
			if ( powerUpType != PowerUpType ) {
				return;
			}
			IsActive             = false;
			_lastTimer           = -1;
			TimerFill.fillAmount = 0f;
			TimerText.text       = "0";
			_anim?.Kill();
			_anim = DOTween.Sequence()
				.Append(GetAnimTo(HidePosition))
				.OnComplete(() => {
					Root.SetActive(false);
					_anim = null;
					OnBecameInactive?.Invoke(this);
				});
		}

		Tween GetAnimTo(Transform dst) {
			return MoveTransform.DOLocalMove(dst.localPosition,
					AnimDuration * Vector2.Distance(ShowPosition.localPosition, HidePosition.localPosition) /
					Vector2.Distance(MoveTransform.localPosition, dst.localPosition))
				.SetEase(Ease.OutSine);
		}

		void SetHidden() {
			Root.SetActive(false);
			MoveTransform.localPosition = HidePosition.localPosition;
		}
	}
}
