using UnityEngine;

using STP.Behaviour.Core.Enemy.GeneratorEditor;
using STP.Config;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core.Generators.Boss {
	public sealed class BoosLevelGeneratorImpl : ILevelGeneratorImpl {
		readonly BossLevelInfo _levelInfo;
		readonly Transform     _levelObjectsRoot;
		readonly Transform     _bordersRoot;
		readonly Player        _player;

		public Rect AreaRect { get; private set; }

		public BoosLevelGeneratorImpl(BossLevelInfo levelInfo, Transform levelObjectsRoot, Transform bordersRoot,
			Player player) {
			_levelInfo        = levelInfo;
			_levelObjectsRoot = levelObjectsRoot;
			_bordersRoot      = bordersRoot;
			_player           = player;
		}

		public UniTask GenerateLevel() {
			var chunkConfig = Resources.Load<ChunkConfig>("ChunkConfig");
			if ( !chunkConfig ) {
				Debug.LogErrorFormat("{0}.{1}: no chunk config", nameof(BoosLevelGeneratorImpl), nameof(GenerateLevel));
				return default;
			}
			var chunkPrefab = chunkConfig.BossChunks[_levelInfo.BossChunkIndex];
			if ( !chunkPrefab ) {
				Debug.LogErrorFormat("{0}.{1}: chunk prefab is null", nameof(BoosLevelGeneratorImpl),
					nameof(GenerateLevel));
				return default;
			}
			var chunkGo = Object.Instantiate(chunkPrefab, _levelObjectsRoot, false);
			var chunk   = chunkGo.GetComponentInChildren<BossChunk>();
			if ( !chunk ) {
				Debug.LogErrorFormat("{0}.{1}: no {2} component in chunk prefab", nameof(BoosLevelGeneratorImpl),
					nameof(GenerateLevel), nameof(BossChunk));
				return default;
			}
			var areaSize = chunk.AreaSize;
			AreaRect = new Rect(-areaSize / 2f, areaSize);

			_player.transform.position = chunk.PlayerSpawnPosition.position;
			_bordersRoot.localScale    = new Vector3(areaSize.x, areaSize.y, 1f);
			return default;
		}
	}
}
