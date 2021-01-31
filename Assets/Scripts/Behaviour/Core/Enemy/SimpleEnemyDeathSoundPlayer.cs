using UnityEngine;

using STP.Behaviour.Utils;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Enemy {
	public sealed class SimpleEnemyDeathSoundPlayer : GameComponent {
		[NotNull] public DelayedDestroyer      DeathSoundDestroyer;
		[NotNull] public Transform             DeathSoundPlayerTransform;
		[NotNull] public BaseSimpleSoundPlayer DeathSoundPlayer;

		Transform _tmpObjRoot;

		public void Init(Transform tmpObjRoot) {
			_tmpObjRoot = tmpObjRoot;
		}

		public void Play() {
			DeathSoundPlayerTransform.SetParent(_tmpObjRoot);
			DeathSoundPlayerTransform.position = transform.position;
			DeathSoundDestroyer.StartDestroy();
			DeathSoundPlayer.Play();
		}
	}
}
