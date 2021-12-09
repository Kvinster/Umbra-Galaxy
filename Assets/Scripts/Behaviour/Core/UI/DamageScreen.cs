using UnityEngine;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using STP.Core;
using DG.Tweening;

namespace STP.Behaviour.Core.UI {
	public sealed class DamageScreen : GameComponent {
		[NotNull] public CanvasGroup DamageFlashVignette;

		public float DamageFlashTime = 0.2f;

		HpSystem _playerHpSystem;

		float _lastHp;

		Sequence _animationSequence;

		public void Init(PlayerController playerController) {
			_playerHpSystem             =  playerController.HpSystem;
			_playerHpSystem.OnHpChanged += OnHpChanged;
			DamageFlashVignette.alpha   =  0f;
			_lastHp                     =  _playerHpSystem.Hp;
			OnHpChanged(_playerHpSystem.Hp);
		}

		public void Deinit() {
			if ( _playerHpSystem != null) {
				_playerHpSystem.OnHpChanged -= OnHpChanged;
			}
			_animationSequence.Kill();
		}

		void OnHpChanged(float newHp) {
			if ( newHp < _lastHp ) {
				TryShowDamageView();
			}
			_lastHp = newHp;
		}

		void TryShowDamageView() {
			if ( _animationSequence.IsActive() ) {
				return;
			}
			DamageFlashVignette.alpha = 0f;
			_animationSequence = DOTween.Sequence()
				.Append(DamageFlashVignette.DOFade(1f, DamageFlashTime))
				.Append(DamageFlashVignette.DOFade(0f, DamageFlashTime));
		}
	}
}
