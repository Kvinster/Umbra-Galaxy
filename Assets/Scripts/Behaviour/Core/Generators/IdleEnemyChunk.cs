using STP.Behaviour.Core.Enemy;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Generators {
	public sealed class IdleEnemyChunk : LevelChunk {
		[NotNull] public EnemyDirector Director;
	}
}