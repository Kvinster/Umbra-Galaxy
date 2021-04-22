using UnityEngine;

using STP.Behaviour.Core.Enemy.GeneratorEditor;

namespace STP.Config {
	[CreateAssetMenu(menuName = "ScriptableObjects/" + nameof(BossLevelInfo), fileName = nameof(BossLevelInfo))]
	public sealed class BossLevelInfo : BaseLevelInfo {
		public int BossChunkIndex;

		public override LevelType LevelType => LevelType.Boss;

		void OnValidate() {
			if ( BossChunkIndex < 0 ) {
				Debug.LogErrorFormat(this, "{0}.{1}: {2} can't be below zero", nameof(BossLevelInfo),
					nameof(OnValidate), nameof(BossChunkIndex));
				return;
			}
			if ( Application.isPlaying ) {
				return;
			}
			var chunksConfig = Resources.Load<ChunkConfig>("ChunkConfig");
			if ( !chunksConfig ) {
				Debug.LogErrorFormat(this, "{0}.{1}: can't load {2}", nameof(BossLevelInfo), nameof(OnValidate),
					nameof(ChunkConfig));
				return;
			}
			if ( BossChunkIndex >= chunksConfig.BossChunks.Count ) {
				Debug.LogErrorFormat("{0}.{1}: {2} is higher than the number of boss chunks in {3}",
					nameof(BossLevelInfo), nameof(OnValidate), nameof(BossChunkIndex), nameof(ChunkConfig));
				return;
			}
		}
	}
}
