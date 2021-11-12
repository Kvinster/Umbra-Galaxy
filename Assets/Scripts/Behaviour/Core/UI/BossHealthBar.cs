using STP.Behaviour.Core.Enemy;
using UnityEngine;

using STP.Behaviour.Core.Enemy.Boss;
using STP.Behaviour.Core.Enemy.BossSpawner;
using STP.Behaviour.Starter;
using STP.Behaviour.Utils.ProgressBar;
using STP.Config;
using STP.Core;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.UI {
	public sealed class BossHealthBar : BaseCoreComponent {
		[NotNull] public GameObject      Root;
		[NotNull] public BaseProgressBar ProgressBar;

		bool      _isBossLevel;
		HpSystem _controllingHpSystem;

		bool IsActive => (_isBossLevel && (_controllingHpSystem != null) && (_controllingHpSystem.Hp > 0)) ;

		void Reset() {
			Root        = gameObject;
			ProgressBar = GetComponentInChildren<BaseProgressBar>();
		}

		protected override void InitInternal(CoreStarter starter) {
			_isBossLevel = (starter.LevelController.CurLevelType == LevelType.Boss);
			
			TrySubscribeToHpChanges(BossController.Instance);
			TrySubscribeToHpChanges(SpawnerBossController.Instance);

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
