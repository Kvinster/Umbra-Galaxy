using STP.Behaviour.Sound;
using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public sealed class MainMenuMusicPlayer : BaseMainMenuComponent {
		[NotNull] public MusicPlayer MusicPlayer;

		protected override void InitInternal(MainMenuStarter starter) {
			MusicPlayer.Play();
		}
	}
}
