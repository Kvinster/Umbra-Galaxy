using UnityEngine;

using STP.Behaviour.Core.Enemy.Boss;
using STP.Behaviour.Starter;
using STP.Behaviour.Utils.ProgressBar;
using STP.Config;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.UI {
	public sealed class BossHealthBar : BaseCoreComponent {
		[NotNull] public GameObject      Root;
		[NotNull] public BaseProgressBar ProgressBar;

		bool           _isBossLevel;
		BossController _boss;

		bool IsActive => (_isBossLevel && _boss);

		void Reset() {
			Root        = gameObject;
			ProgressBar = GetComponentInChildren<BaseProgressBar>();
		}

		protected override void InitInternal(CoreStarter starter) {
			_isBossLevel = (starter.LevelController.CurLevelType == LevelType.Boss);
			_boss        = BossController.Instance;

			if ( _boss ) {
				_boss.OnCurHpChanged += OnBossCurHpChanged;
			}

			UpdateView();
		}

		void UpdateView() {
			Root.SetActive(IsActive);
			if ( IsActive ) {
				UpdateProgress();
			}
		}

		void OnBossCurHpChanged(float _) {
			UpdateProgress();
		}

		void UpdateProgress() {
			if ( !_boss ) {
				return;
			}
			ProgressBar.Progress = _boss.CurHp / _boss.MaxHp;
		}
	}
}
