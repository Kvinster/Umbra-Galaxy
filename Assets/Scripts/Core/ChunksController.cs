using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Enemy;
using STP.Config;
using STP.Core.State;

namespace STP.Core {
	public sealed class ChunkController : BaseStateController {
		readonly ChunkConfig _chunkConfig;

		readonly Dictionary<string, int> _chunkGeneratorsCount = new Dictionary<string, int>();

		public ChunkController(GameState gameState) : base(gameState) {
			_chunkConfig = LoadConfig();
			Debug.Assert(_chunkConfig);
		}

		public GameObject GetChunkPrefab(string chunkName) {
			return _chunkConfig.GetChunk(chunkName);
		}

		public bool HasChunk(string chunkName) {
			return _chunkConfig.GetChunk(chunkName);
		}

		public int GetGeneratorsCountInChunk(string chunkName) {
			if ( _chunkGeneratorsCount.ContainsKey(chunkName) ) {
				return _chunkGeneratorsCount[chunkName];
			}
			if ( !HasChunk(chunkName) ) {
				Debug.LogError($"Can't get chunk info from config. Chunk with name {chunkName} was not found");
				return 0;
			}
			var chunkInfo  = _chunkConfig.GetChunk(chunkName);
			var generators = chunkInfo.GetComponentsInChildren<Generator>();
			var count      = 0;
			foreach ( var generator in generators ) {
				if ( generator.IsMainGenerator ) {
					count++;
				}
			}
			_chunkGeneratorsCount.Add(chunkName, count);
			return count;
		}

		public string GetMinChunkWithGeneratorsCountHigherThan(int genCount) {
			var minGenerationCount = int.MaxValue;
			var minChunkName       = string.Empty;
			foreach ( var chunkInfo in _chunkConfig.ChunkInfos ) {
				var generationCount = GetGeneratorsCountInChunk(chunkInfo.Name);
				if ( (minGenerationCount > generationCount) && (generationCount > genCount) ) {
					minChunkName       = chunkInfo.Name;
					minGenerationCount = generationCount;
				}
			}
			if ( minGenerationCount == int.MaxValue ) {
				Debug.LogError($"Can't find chunk with higher generation count than {genCount}");
				return string.Empty;
			}
			return minChunkName;
		}

		public static GameObject GetChunkPrefabInEditor(string chunkName) {
			var config = LoadConfig();
			if ( config ) {
				return config.GetChunk(chunkName);
			}
			Debug.LogError("Can't load chunk config in editor");
			return null;
		}

		public static int GetGeneratorsCountInChunkInEditor(string chunkName) {
			var chunkConfig     = LoadConfig();
			var chunkInfo       = chunkConfig.GetChunk(chunkName);
			var generatorsCount = chunkInfo.GetComponentsInChildren<Generator>().Length;
			return generatorsCount;
		}

		static ChunkConfig LoadConfig() {
			return Resources.Load<ChunkConfig>("ChunkConfig");
		}
	}
}
