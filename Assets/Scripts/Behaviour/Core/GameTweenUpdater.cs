using DG.Tweening;
using STP.Behaviour.Starter;
using STP.Manager;
using UnityEngine;

namespace STP.Behaviour.Core {
	public sealed class GameTweenUpdater : BaseCoreComponent {
		PauseManager _pauseManager;

		void Update() {
			if ( _pauseManager?.IsPaused ?? false ) {
				return;
			}
			DOTween.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
		}
		
		protected override void InitInternal(CoreStarter starter) {
			_pauseManager = starter.PauseManager;
		}
	}
}