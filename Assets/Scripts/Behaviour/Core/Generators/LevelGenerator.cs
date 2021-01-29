using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Common;
using STP.Config;
using STP.Core;
using STP.Utils;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core.Generators {
	public sealed class LevelGenerator : GameComponent {

		LevelGeneratorState _state = new LevelGeneratorState();

		ChunkController   _chunkController;
		PowerUpController _powerUpController;

		public void Init(ChunkController chunkController, PowerUpController powerUpController) {
			_chunkController   = chunkController;
			_powerUpController = powerUpController;
		}

		public void GenerateLevel(LevelInfo levelInfo, Func<string, GameObject> prefabGetter, Transform root = null) {
			ResetState();
			var cellSize   = levelInfo.CellSize;
			var randomSeed = Random.Range(int.MinValue, int.MaxValue);
			Random.InitState(randomSeed);
			var minPoint          = new Vector2(-levelInfo.LevelSpaceSize / 2.0f, -levelInfo.LevelSpaceSize / 2.0f) + new Vector2(cellSize / 2.0f, cellSize / 2.0f);
			var map               = GenerateMap(levelInfo);
			var powerUpSpawnPoints = new List<Transform>();
			for ( var y = 0; y < map.GetLength(1); y++ ) {
				for ( var x = 0; x < map.GetLength(0); x++ ) {
					var prefab    = prefabGetter(map[x, y]);
					var pos       = (Vector3)minPoint + new Vector3(x * cellSize, y * cellSize);
					var instance  = Instantiate(prefab, pos, Quaternion.identity, root);
					var chunkComp = instance.GetComponent<LevelChunk>();
					powerUpSpawnPoints.AddRange(chunkComp.FreePowerUpSpawnPoints);
				}
			}

			foreach ( var powerUpInfo in levelInfo.PowerUpInfos ) {
				while ( (_state.GetOrDefaultCreatedPowerUpsCount(powerUpInfo.PowerUpType) < powerUpInfo.Count) && (powerUpSpawnPoints.Count > 0) ) {
					AddPowerUpsToLevel(powerUpSpawnPoints, levelInfo, root);
				}
			}
			Debug.Log($"Level generated. Seed {randomSeed}");
		}

		void ResetState() {
			_state = new LevelGeneratorState();
		}

		string[,] GenerateMap(LevelInfo levelInfo) {
			var sideCellsCount   = levelInfo.LevelSpaceSize / levelInfo.CellSize;
			var map              = new string[sideCellsCount, sideCellsCount];
			var neededGenerators = levelInfo.GeneratorsCount;

			var cells = new List<Vector2Int>();
			for ( var y = 0; y < sideCellsCount; y++ ) {
				for ( var x = 0; x < sideCellsCount; x++ ) {
					cells.Add(new Vector2Int(x, y));
				}
			}

			//Reserve one rect for safe area
			var safeRectCoords = new Vector2Int(sideCellsCount / 2, sideCellsCount / 2);
			cells.Remove(safeRectCoords);
			map[safeRectCoords.x, safeRectCoords.y] = ChunkConfig.SafeRectName;

			for ( var i = cells.Count; i > 0; i-- ) {
				var randIndex = Random.Range(0, i);
				var mapIndex  = cells[randIndex];
				cells.RemoveAt(randIndex);
				map[mapIndex.x, mapIndex.y] = GetRandomChunk(levelInfo, neededGenerators);

				neededGenerators -= _chunkController.GetGeneratorsCountInChunk(map[mapIndex.x, mapIndex.y]);
			}

			if ( neededGenerators != 0 ) {
				Debug.LogWarning($"Not all generators were created. Needed more {neededGenerators} generators. Changing some chunks for fixing it");
				TryRaiseDifficultyInChunks(map, neededGenerators);
			}

			return map;
		}

		void TryRaiseDifficultyInChunks(string[,] map, int moreNeededGenerators) {
			var leftGenerators     = moreNeededGenerators;
			if ( map.GetLength(0) <= 1 ) {
				Debug.LogErrorFormat("Can't raise difficulty - space sections count is too low {0}", map.GetLength(0));
				return;
			}
			while ( leftGenerators > 0 ) {
				// Find easiest chunks
				var minGeneratorsCount = int.MaxValue;
				var minChunkName       = string.Empty;
				for ( var y = 0; y < map.GetLength(1); y++ ) {
					for ( var x = 0; x < map.GetLength(0); x++ ) {
						var generatorsCountInCell = _chunkController.GetGeneratorsCountInChunk(map[x, y]);
						if ( minGeneratorsCount > generatorsCountInCell && (map[x, y] != ChunkConfig.SafeRectName) ) {
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
				// Get all easiest chunks on map
				var generationsCountDiff = newChunkGeneratorsCount - minGeneratorsCount;
				var minChunks            = new List<(int x, int y)>();
				for ( var y = 0; y < map.GetLength(1); y++ ) {
					for ( var x = 0; x < map.GetLength(0); x++ ) {
						if ( map[x, y] == minChunkName) {
							minChunks.Add((x, y));
						}
					}
				}
				// Change easiest chunks with more difficult
				while ( (leftGenerators > 0) && (minChunks.Count > 0) ) {
					var randomChunkCoords = RandomUtils.GetAndRemoveRandomElement(minChunks);
					map[randomChunkCoords.x, randomChunkCoords.y] =  chunkWithHigherDifficulty;
					leftGenerators -= generationsCountDiff;
				}
			}
		}

		string GetRandomChunk(LevelInfo levelInfo, int neededGenerators) {
			var availableChunks = levelInfo.Chunks.FindAll((x) => IsChunkAvailable(x, neededGenerators));
			return RandomUtils.GetRandomWeightedElement(availableChunks)?.Name;
		}

		void AddPowerUpsToLevel(List<Transform> freePowerUpsSpawnPoints, LevelInfo levelInfo, Transform root) {
			var powerUpSpawn = RandomUtils.GetRandomElement(freePowerUpsSpawnPoints);
			if ( !powerUpSpawn ) {
				Debug.LogError("Can't spawn power up - no available spawn points");
				return;
			}
			freePowerUpsSpawnPoints.Remove(powerUpSpawn);
			var availablePowerUps = GetAvailableToSpawnPowerUps(levelInfo);
			var powerUp           = RandomUtils.GetRandomElement(availablePowerUps);
			var prefab            = GetPowerUpPrefab(powerUp);
			Instantiate(prefab, powerUpSpawn.position, Quaternion.identity, root);
			if ( _state.CreatedPowerUpsCount.ContainsKey(powerUp) ) {
				_state.CreatedPowerUpsCount[powerUp]++;
			} else {
				_state.CreatedPowerUpsCount[powerUp] = 1;
			}
		}

		List<PowerUpType> GetAvailableToSpawnPowerUps(LevelInfo levelInfo) {
			var res             = new List<PowerUpType>();
			var createdPowerUps = _state.CreatedPowerUpsCount;
			foreach ( var powerUpInfo in levelInfo.PowerUpInfos ) {
				var type = powerUpInfo.PowerUpType;
				if ( !createdPowerUps.ContainsKey(type) || (createdPowerUps[type] < powerUpInfo.Count) ) {
					res.Add(type);
				}
			}
			return res;
		}

		GameObject GetPowerUpPrefab(PowerUpType powerUpType) {
			return _powerUpController.GetPowerUpPrefab(powerUpType);
		}


		bool IsChunkAvailable(ChunkWeightInfo chunkWeightInfo, int needGenerators) {
			return _chunkController.GetGeneratorsCountInChunk(chunkWeightInfo.Name) <= needGenerators;
		}
	}
}