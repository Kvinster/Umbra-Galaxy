using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

using System.Threading;

using STP.Behaviour.Starter;
using STP.Common;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core {
	public sealed class PlayerDamagePowerUpView : BaseCoreComponent {
		[NotNull] public GameObject   Root;
		[NotNull] public VisualEffect VisualEffect;

		PlayerManager _playerManager;

		CancellationTokenSource _cancellationTokenSource;

		protected override void OnDisable() {
			base.OnDisable();

			if ( _playerManager != null ) {
				_playerManager.OnPowerUpStarted  -= OnPowerUpStarted;
				_playerManager.OnPowerUpFinished -= OnPowerUpFinished;
			}

			_cancellationTokenSource.Cancel();
		}

		protected override void InitInternal(CoreStarter starter) {
			_playerManager                   =  starter.PlayerManager;
			_playerManager.OnPowerUpStarted  += OnPowerUpStarted;
			_playerManager.OnPowerUpFinished += OnPowerUpFinished;
		}

		void OnPowerUpStarted(PowerUpType powerUpType) {
			if ( powerUpType != PowerUpType.X2Damage ) {
				return;
			}
			if ( _cancellationTokenSource != null ) {
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource = null;
			}
			Root.SetActive(true);
			VisualEffect.Play();
		}

		void OnPowerUpFinished(PowerUpType powerUpType) {
			if ( powerUpType != PowerUpType.X2Damage ) {
				return;
			}
			StopEffectAndDisable().Forget();
		}

		async UniTaskVoid StopEffectAndDisable() {
			Assert.IsNull(_cancellationTokenSource);
			_cancellationTokenSource = new CancellationTokenSource();
			VisualEffect.Stop();
			await UniTask.WaitWhile(() => VisualEffect.aliveParticleCount > 0, PlayerLoopTiming.Update,
				_cancellationTokenSource.Token);
			if ( _cancellationTokenSource?.IsCancellationRequested ?? true ) {
				return;
			}
			Root.SetActive(false);
		}
	}
}
