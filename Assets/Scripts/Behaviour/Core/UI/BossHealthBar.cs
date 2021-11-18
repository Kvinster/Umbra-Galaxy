﻿using STP.Behaviour.Core.Enemy;
using UnityEngine;
using STP.Behaviour.Core.Enemy.BossSpawner;
using STP.Behaviour.Utils.ProgressBar;
using STP.Config;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.UI {
	public sealed class BossHealthBar : GameComponent {
		[NotNull] public GameObject      Root;
		[NotNull] public BaseProgressBar ProgressBar;

		bool      _isBossLevel;
		HpSystem _controllingHpSystem;

		bool IsActive => (_isBossLevel && (_controllingHpSystem != null) && (_controllingHpSystem.Hp > 0)) ;

		void Reset() {
			Root        = gameObject;
			ProgressBar = GetComponentInChildren<BaseProgressBar>();
		}

		public void Init(LevelController levelController) {
			_isBossLevel = (levelController.CurLevelType == LevelType.Boss);

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
