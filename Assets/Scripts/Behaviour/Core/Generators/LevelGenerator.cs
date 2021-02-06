using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using STP.Behaviour.Core.Enemy.GeneratorEditor;
using STP.Common;
using STP.Config;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Generators {
	public sealed class LevelGenerator : GameComponent {
		[NotNull] public ChunkCreator Creator;

		LevelGeneratorState _state = new LevelGeneratorState();

		PowerUpController _powerUpController;
		LevelController   _levelController;

		public void Init(PowerUpController powerUpController, LevelController levelController) {
			_powerUpController = powerUpController;
			_levelController   = levelController;
		}

		public IEnumerator GenerateLevel(Transform root = null) {
			ResetState();
			var levelInfo  = _levelController.GetCurLevelConfig();
			var cellSize   = levelInfo.CellSize;
			var randomSeed = Random.Range(int.MinValue, int.MaxValue);
			Random.InitState(randomSeed);
			var minPoint          = new Vector2(-levelInfo.LevelSpaceSize / 2.0f, -levelInfo.LevelSpaceSize / 2.0f) + new Vector2(cellSize / 2.0f, cellSize / 2.0f);
			var map               = GenerateMap(levelInfo);
			var powerUpSpawnPoints = new List<Transform>();
			for ( var y = 0; y < map.GetLength(1); y++ ) {
				for ( var x = 0; x < map.GetLength(0); x++ ) {
					var mapCell = map[x, y];
					var obj     = GetChunk(levelInfo, mapCell);
					var pos     = (Vector3) minPoint + new Vector3(x * cellSize, y * cellSize);
					obj.transform.SetParent(root);
					obj.transform.position = pos;
					var chunkComp = obj.GetComponent<LevelChunk>();
					powerUpSpawnPoints.AddRange(chunkComp.FreePowerUpSpawnPoints);
				}
				yield return null;
			}

			foreach ( var powerUpInfo in levelInfo.PowerUpInfos ) {
				while ( (_state.GetOrDefaultCreatedPowerUpsCount(powerUpInfo.PowerUpType) < powerUpInfo.Count) &&
				        (powerUpSpawnPoints.Count > 0) ) {
					AddPowerUpsToLevel(powerUpSpawnPoints, levelInfo, root);
				}
			}
			Debug.Log($"Level generated. Seed {randomSeed}");
		}

		void ResetState() {
			_state = new LevelGeneratorState();
		}

		GameObject GetChunk(LevelInfo levelInfo, MapCell mapCell) {
			var chunkInfo = RandomUtils.GetRandomElement(levelInfo.Chunks);

			if ( mapCell.IsSafeRect ) {
				return Instantiate(Creator.SafeAreaPrefab);
			}
			return Creator.CreateGeneratorChunk(mapCell.GeneratorGridSize, chunkInfo.PowerUpCount);
		}

		MapCell[,] GenerateMap(LevelInfo levelInfo) {
			var sideCellsCount   = levelInfo.LevelSpaceSize / levelInfo.CellSize;
			var map              = new MapCell[sideCellsCount, sideCellsCount];
			var neededGenerators = levelInfo.GeneratorsCount;

			var cells = new List<Vector2Int>();
			for ( var y = 0; y < sideCellsCount; y++ ) {
				for ( var x = 0; x < sideCellsCount; x++ ) {
					cells.Add(new Vector2Int(x, y));
				}
			}

			// Reserve one rect for safe area
			var safeRectCoords = new Vector2Int(sideCellsCount / 2, sideCellsCount / 2);
			cells.Remove(safeRectCoords);
			map[safeRectCoords.x, safeRectCoords.y] = new MapCell { IsSafeRect = true, GeneratorGridSize = 0};

			for ( var i = cells.Count; i > 0; i-- ) {
				var randIndex = Random.Range(0, i);
				var mapIndex  = cells[randIndex];
				cells.RemoveAt(randIndex);
				if ( neededGenerators == 0 ) {
					map[mapIndex.x, mapIndex.y] = new MapCell { IsSafeRect = false, GeneratorGridSize = 0};
				} else {
					map[mapIndex.x, mapIndex.y] = GetRandomChunk(levelInfo);
					if ( map[mapIndex.x, mapIndex.y].GeneratorGridSize != 0 ) {
						neededGenerators--;
					}
				}
			}

			if ( neededGenerators != 0 ) {
				Debug.LogWarning($"Not all generators were created. Needed more {neededGenerators} generators. Changing some chunks for fixing it");
				TryRaiseDifficultyInChunks(map, neededGenerators);
			}

			return map;
		}

		void TryRaiseDifficultyInChunks(MapCell[,] map, int moreNeededGenerators) {
			var leftGenerators = moreNeededGenerators;
			if ( map.GetLength(0) <= 1 ) {
				Debug.LogErrorFormat("Can't raise difficulty - space sections count is too low {0}", map.GetLength(0));
				return;
			}
			while ( leftGenerators > 0 ) {
				// Get all easiest chunks on map
				var minChunks = new List<(int x, int y)>();
				for ( var y = 0; y < map.GetLength(1); y++ ) {
					for ( var x = 0; x < map.GetLength(0); x++ ) {
						if ( (map[x, y].GeneratorGridSize == 0) && !map[x, y].IsSafeRect ) {
							minChunks.Add((x, y));
						}
					}
				}

				// Change easiest chunks with more difficult
				while ( (leftGenerators > 0) && (minChunks.Count > 0) ) {
					var randomChunkCoords = RandomUtils.GetAndRemoveRandomElement(minChunks);
					map[randomChunkCoords.x, randomChunkCoords.y].GeneratorGridSize = Random.Range(7, 10);
					leftGenerators--;
				}
			}
		}

		MapCell GetRandomChunk(LevelInfo levelInfo) {
			var gridSize = RandomUtils.GetRandomWeightedElement(levelInfo.Chunks)?.GeneratorGridSize ?? 0;
			return new MapCell {IsSafeRect = false, GeneratorGridSize = gridSize};
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
	}
}