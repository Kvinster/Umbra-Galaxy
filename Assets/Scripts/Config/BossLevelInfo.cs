using UnityEngine;

namespace STP.Config {
	[CreateAssetMenu(menuName = "ScriptableObjects/" + nameof(BossLevelInfo), fileName = nameof(BossLevelInfo))]
	public sealed class BossLevelInfo : BaseLevelInfo {
		public int BossChunkIndex;

		public override LevelType LevelType => LevelType.Boss;

		void OnValidate() {
			if ( BossChunkIndex < 0 ) {
				Debug.LogErrorFormat(this, "{0}.{1}: {2} can't be below zero", nameof(BossLevelInfo),
					nameof(OnValidate), nameof(BossChunkIndex));
			}
		}
	}
}
