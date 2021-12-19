using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Utils.ProgressBar;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;
using UnityEngine;

namespace STP.Behaviour.Core.UI {
	public sealed class BossHealthBar : GameComponent {
		[NotNull] public GameObject      Root;
		[NotNull] public BaseProgressBar ProgressBar;

		BaseBoss _baseBoss;

		HpSystem _controllingHpSystem;

		bool IsActive => (_baseBoss && (_controllingHpSystem != null) && (_controllingHpSystem.Hp > 0)) ;

		void Reset() {
			Root        = gameObject;
			ProgressBar = GetComponentInChildren<BaseProgressBar>();
		}

		public void Init(BaseBoss baseBoss) {
			_baseBoss = baseBoss;

			TrySubscribeToHpChanges(baseBoss);

			UpdateView();
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