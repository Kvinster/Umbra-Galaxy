using UnityEngine;
using UnityEngine.VFX;

using System;
using System.Threading;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace STP.Behaviour.Core.Enemy {
	public sealed class SimpleEnemyDeathEffect : GameComponent {
		const string StartEventName = "Start";

		[Range(0.1f, 4f)]
		public float Lifetime;

		[NotNull] public Transform    Target;
		[NotNull] public VisualEffect Effect;

		void OnEnable() {
			Effect.SetFloat("Lifetime", Lifetime);
			Effect.Stop();
		}

		public async UniTask Play(CancellationToken cancellationToken = default) {
			Effect.SendEvent(StartEventName);
			await UniTask.Delay(TimeSpan.FromSeconds(Lifetime * 0.9f), cancellationToken: cancellationToken);
			await Target.DOScale(Vector3.zero, Lifetime * 0.1f).WithCancellation(cancellationToken);
		}
	}
}
