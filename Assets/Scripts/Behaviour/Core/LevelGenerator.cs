using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Controller;
using STP.Utils;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core {
	public class LevelGenerator : GameComponent {
		[Serializable]
		public class Chunk {
			public int        Weight;
			public GameObject Prefab;
		}

		public int CellSideSize  = 300;
		public int SpaceSideSize = 10000;

		public List<Chunk> LevelChunks;


		public void GenerateLevel(LevelController levelController) {
			var levelConfig = levelController.GetCurLevelConfig();
			var minPoint    = new Vector2(-SpaceSideSize / 2.0f, -SpaceSideSize / 2.0f);
			var maxPoint    = new Vector2( SpaceSideSize / 2.0f,  SpaceSideSize / 2.0f);
			Random.InitState(levelConfig.LevelSeed);
			for ( var y = minPoint.y; y < maxPoint.y; y += CellSideSize ) {
				for ( var x = minPoint.x; x < maxPoint.x; x += CellSideSize ) {
					var chunk = GetRandomChunk();
					Instantiate(chunk, new Vector3(x, y, 0), Quaternion.identity);
				}
			}
		}

		GameObject GetRandomChunk() {
			var totalWeight = 0;
			foreach ( var chunk in LevelChunks ) {
				totalWeight += chunk.Weight;
			}

			var randomValue = Random.Range(0, totalWeight);
			foreach ( var chunk in LevelChunks ) {
				if ( chunk.Weight > randomValue ) {
					return chunk.Prefab;
				}

				randomValue -= chunk.Weight;
			}

			return null;
		}
	}
}