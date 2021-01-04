using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Config {
	[CreateAssetMenu(fileName = "ChunkConfig", menuName = "ScriptableObjects/ChunkConfig", order = 1)]
	public class ChunkConfig : ScriptableObject {
		[Serializable]
		public class ChunkInfo {
			public string     Name;
			public GameObject Prefab;
		}

		public List<ChunkInfo> ChunkInfos;

		public GameObject GetChunk(string chunkName) {
			return GetChunkInfo(chunkName)?.Prefab;
		}

		ChunkInfo GetChunkInfo(string chunkName) {
			return ChunkInfos.Find(x => x.Name == chunkName);
		}
 	}
}