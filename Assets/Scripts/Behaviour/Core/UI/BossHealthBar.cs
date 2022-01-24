using UnityEngine;

using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Utils.ProgressBar;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

namespace STP.Behaviour.Core.UI {
	public sealed class BossHealthBar : GameComponent {
		[NotNull] public GameObject      Root;
		[NotNull] public BaseProgressBar ProgressBar;
		[Header("Appear anim parameters")]
		public float StartDelay;
		public float AnimDuration;
		[Header("Appear anim dependencies")]
		[NotNull] public Transform AnimTarget;
		[NotNull] public Transform HidePos;
		[NotNull] public Transform ShowPos;

		BaseBoss _baseBoss;

		HpSystem _controllingHpSystem;

		bool     _needPlayAnim;
		Sequence _anim;

		bool IsActive => (_baseBoss && (_controllingHpSystem != null) && (_controllingHpSystem.Hp > 0)) ;

		void Reset() {
			Root        = gameObject;
			ProgressBar = GetComponentInChildren<BaseProgressBar>();
		}

		void OnDisable() {
			_anim?.Kill();
		}

		void OnEnable() {
			if ( _needPlayAnim ) {
				PlayAppearAnim();
				_needPlayAnim = false;
			}
		}

		public void Init(BaseBoss baseBoss) {
			_baseBoss = baseBoss;

			TrySubscribeToHpChanges(baseBoss);

			UpdateView();
			TryPlayAppearAnim();
		}

		void TrySubscribeToHpChanges(IHpSource hpSource) {
			if ( hpSource == null ) {
				return;
			}
			_controllingHpSystem             =  hpSource.HpSystemComponent;
			_controllingHpSystem.OnHpChanged += OnBossCurHpChanged;
			_controllingHpSystem.OnDied      += OnDied;
		}

		void UpdateView() {
			Root.SetActive(IsActive);
			if ( IsActive ) {
				UpdateProgress();
			}
		}

		void TryPlayAppearAnim() {
			if ( !_baseBoss ) {
				return;
			}
			_needPlayAnim            = true;
			AnimTarget.localPosition = HidePos.localPosition;
		}

		void PlayAppearAnim() {

			_anim = DOTween.Sequence()
				.AppendInterval(StartDelay)
				.Append(AnimTarget.DOLocalMove(ShowPos.localPosition, AnimDuration))
				.OnComplete(() => _anim = null)
				.SetUpdate(UpdateType.Manual);
		}

		void OnDied() {
			UpdateView();
			_controllingHpSystem.OnHpChanged -= OnBossCurHpChanged;
			_controllingHpSystem.OnDied      -= OnDied;
		}

		void OnBossCurHpChanged(float _) {
			UpdateProgress();
		}

		void UpdateProgress() {
			if ( _controllingHpSystem == null  ) {
				return;
			}
			ProgressBar.Progress = _controllingHpSystem.Hp / _controllingHpSystem.MaxHp;
		}
	}
}