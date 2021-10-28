using UnityEngine;

namespace STP.Config {
	[CreateAssetMenu(menuName = "ScriptableObjects/" + nameof(BossLevelInfo), fileName = nameof(BossLevelInfo))]
	public sealed class BossLevelInfo : BaseLevelInfo {
		public override LevelType LevelType => LevelType.Boss;
	}
}
