using STP.Behaviour.Sound;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	public sealed class CoreMusicPlayer : BaseCoreComponent {
		[NotNull] public MusicPlayer MusicPlayer;

		protected override void InitInternal(CoreStarter starter) {
			MusicPlayer.Play();
		}
	}
}
