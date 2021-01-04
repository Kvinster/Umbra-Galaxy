using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Config;
using STP.Controller;
using STP.Utils;

using JetBrains.Annotations;
using NaughtyAttributes;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core.LevelGeneration {
	public class LevelGenerator : GameComponent {
		[Serializable]
		public class ChunkWeightInfo {
			public int    Weight;
			public string Name;
		}

		public int CellSideSize  = 300;

		public List<ChunkWeightInfo> LevelChunks;

		[Header("for editor button")] 
		public Transform Root;

		LevelController _levelController;
		ChunkController _chunkController;
		
		[Button("Try generate level")] [UsedImplicitly]
		void GenerateLevelInEditor() {
			var randomSeed = Random.Range(int.MinValue, int.MaxValue);
			Random.InitState(randomSeed);
			for ( var i = Root.childCount-1; i >= 0; i-- ) {
				DestroyImmediate(Root.GetChild(i).gameObject);
			}
			Init(LevelController.Instance, ChunkController.Instance);
			GenerateLevel(Root);
			Debug.Log($"Level generated. Seed {randomSeed}");
		}

		public void Init(LevelController levelController, ChunkController chunkController) {
			_levelController = levelController;
			_chunkController = chunkController;
		}
		
		public void GenerateLevel(Transform root = null) {
			var levelInfo = _levelController.GetCurLevelConfig();
			var minPoint = new Vector2(-levelInfo.LevelSpaceSize / 2.0f, -levelInfo.LevelSpaceSize / 2.0f);
			var map      = GenerateMap(levelInfo);
			for ( var y = 0; y < map.GetLength(1); y++ ) {
				for ( var x = 0; x < map.GetLength(0); x++ ) {
					var prefab = _chunkController.GetChunkPrefab(map[x, y]);
					var pos    = (Vector3)minPoint + new Vector3(x * CellSideSize, y * CellSideSize);
					Instantiate(prefab, pos, Quaternion.identity, root);
				}
			}
		}

		string[,] GenerateMap(LevelInfo levelInfo) {
			var sideCellsCount   = levelInfo.LevelSpaceSize / CellSideSize;
			var res              = new string[sideCellsCount, sideCellsCount];
			var neededGenerators = _levelController.GetCurLevelConfig().GeneratorsCount;

			var cells = new List<Vector2Int>();
			for ( var y = 0; y < sideCellsCount; y++ ) {
				for ( var x = 0; x < sideCellsCount; x++ ) {
					cells.Add(new Vector2Int(x, y));
				}
			}
			for ( var i = cells.Count; i > 0; i-- ) {
				var randIndex = Random.Range(0, i);
				var mapIndex  = cells[randIndex];
				cells.RemoveAt(randIndex);
				res[mapIndex.x, mapIndex.y] = GetRandomChunk(neededGenerators);
				neededGenerators           -= _chunkController.GetGeneratorsCountInChunk(res[mapIndex.x, mapIndex.y]);
			}
			if ( neededGenerators != 0 ) {
				Debug.LogWarning($"Not all generators were created. Needed more {neededGenerators} generators. Changing some chunks for fixing it");	
				TryRaiseDifficultyInChunks(res, neededGenerators);
			}

			return res;
		}

		void TryRaiseDifficultyInChunks(string[,] map, int moreNeededGenerators) {
			var leftGenerators     = moreNeededGenerators;
			while ( leftGenerators > 0 ) {
				// Find easiest chunks
				var minGeneratorsCount = int.MaxValue;
				var minChunkName       = string.Empty;
				for ( var y = 0; y < map.GetLength(1); y++ ) {
					for ( var x = 0; x < map.GetLength(0); x++ ) {
						var generatorsCountInCell = _chunkController.GetGeneratorsCountInChunk(map[x, y]);
						if ( minGeneratorsCount > generatorsCountInCell ) {
							minGeneratorsCount = generatorsCountInCell;
							minChunkName       = map[x, y];
						}
					}
				}
				// Get more difficult chunk
				var chunkWithHigherDifficulty = _chunkController.GetMinChunkWithGeneratorsCountHigherThan(minGeneratorsCount);
				var newChunkGeneratorsCount   = _chunkController.GetGeneratorsCountInChunk(chunkWithHigherDifficulty);
				if ( string.IsNullOrEmpty(chunkWithHigherDifficulty) ) {
					Debug.LogError("Can't find more difficult chunk => can't raise a difficulty");
					return;
				}
				// Change easiest chunks with more difficult
				var generationsCountDiff = newChunkGeneratorsCount - minGeneratorsCount;
				for ( var y = 0; y < map.GetLength(1); y++ ) {
					for ( var x = 0; x < map.GetLength(0); x++ ) {
						if ( map[x, y] == minChunkName) {
							map[x, y]      =  chunkWithHigherDifficulty;
							leftGenerators -= generationsCountDiff;
							if ( leftGenerators <= 0 ) {
								return;
							}
						}
					}
				}
			}
		}
		
		string GetRandomChunk(int neededGenerators) {
			var totalWeight     = 0;
			var availableChunks = LevelChunks.FindAll((x) => IsChunkAvailable(x, neededGenerators));
			foreach ( var chunk in availableChunks ) {
				totalWeight += chunk.Weight;
			}
			var randomValue = Random.Range(0, totalWeight);
			foreach ( var chunk in availableChunks ) {
				if ( chunk.Weight > randomValue ) {
					return chunk.Name;
				}
				randomValue -= chunk.Weight;
			}
			return null;
		}

		bool IsChunkAvailable(ChunkWeightInfo chunkWeightInfo, int needGenerators) {
			return _chunkController.GetGeneratorsCountInChunk(chunkWeightInfo.Name) <= needGenerators;
		}
	}
}