using System.Collections.Generic;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.MainMenu {
	public class MainLevelUiContainer : GameComponent {
		[NotNull] public LevelButton       MainLevel;
		[NotNull] public List<LevelButton> OptionalLevels;
	}
}