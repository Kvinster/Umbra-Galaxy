using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Enemy;
using STP.Config;
using STP.Utils;

namespace STP.Core {
	public class ChunkController : Singleton<ChunkController> {
		ChunkConfig _chunkConfig;

		readonly Dictionary<string, int> _chunkGeneratorsCount = new Dictionary<string, int>();

		public ChunkController() {
			LoadConfig();
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
			var chunkInfo       = _chunkConfig.GetChunk(chunkName);
			var generatorsCount = chunkInfo.GetComponentsInChildren<Generator>().Length;
			_chunkGeneratorsCount.Add(chunkName, generatorsCount);
			return generatorsCount;
		}

		void LoadConfig() {
			_chunkConfig = Resources.Load<ChunkConfig>("ChunkConfig");
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
	}
}