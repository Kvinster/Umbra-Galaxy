using UnityEngine;

using STP.Behaviour.Sound;
using STP.Behaviour.Starter;
using STP.Config;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class CoreMusicPlayer : BaseCoreComponent {
		[Header("Parameters")]
		public bool DisableOnBossLevel;
		[Header("Dependencies")]
		[NotNull] public MusicPlayer MusicPlayer;

		protected override void InitInternal(CoreStarter starter) {
			if ( DisableOnBossLevel && (starter.LevelController.CurLevelType == LevelType.Boss) ) {
				return;
			}
			MusicPlayer.Play();
		}
	}
}
