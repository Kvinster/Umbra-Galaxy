using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Core.Enemy.GeneratorEditor;
using STP.Behaviour.Starter;
using STP.Common;
using STP.Config;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core.Generators {
	public sealed class LevelGenerator : GameComponent {
		const int PowerUpPlacesCountInGeneratorChunk = 3;
		const int MinSpaceBetweenGenerators = 300;

		[NotNull] public ChunkCreator Creator;
		[NotNull] public Transform    BordersRoot;

		LevelGeneratorState _state = new LevelGeneratorState();
		RegularLevelInfo    _levelInfo;

		PrefabsController _prefabsController;

		public async UniTask GenerateLevel(PrefabsController prefabsController, LevelController levelController, CoreStarter starter, Transform root = null) {
			var randomSeed = Random.Range(int.MinValue, int.MaxValue);
			Random.InitState(randomSeed);
			ResetState();
			_prefabsController = prefabsController;
			_levelInfo         = levelController.GetCurLevelConfig();
			UpdateMapSizesInState();
			var map            = GenerateMap();
			var levelSpaceSize = _state.LevelSideBlocksCount * _state.LevelChunkSideSize;
			var minPoint = new Vector2(-levelSpaceSize / 2.0f, -levelSpaceSize / 2.0f) +
			               new Vector2(_state.LevelChunkSideSize / 2.0f, _state.LevelChunkSideSize / 2.0f);
			var powerUpSpawnPoints = new List<Transform>();
			// Create map chunks
			for ( var y = 0; y < map.GetLength(1); y++ ) {
				for ( var x = 0; x < map.GetLength(0); x++ ) {
					var mapCell = map[x, y];
					var obj     = GetChunk(mapCell);
					var pos     = (Vector3) minPoint + new Vector3(x * _state.LevelChunkSideSize, y * _state.LevelChunkSideSize);
					obj.transform.SetParent(root);
					obj.transform.position = pos;
					var chunkComp = obj.GetComponent<LevelChunk>();
					if ( chunkComp is IdleEnemyChunk idleChunk ) {
						var controllableEnemies = idleChunk.GetComponentsInChildren<BaseControllableEnemy>();
						idleChunk.Director.Init(starter.Player, new List<BaseControllableEnemy>(controllableEnemies));
					}
					powerUpSpawnPoints.AddRange(chunkComp.FreePowerUpSpawnPoints);
				}
				await UniTask.Yield();
			}
			// Create powerups
			foreach ( var powerUp in _levelInfo.PowerUps ) {
				AddPowerUpsToLevel(powerUpSpawnPoints, powerUp);
			}
			// Init player camera follower
			var areaSize = new Vector2(_state.LevelChunkSideSize * (map.GetLength(0) + 1) , _state.LevelChunkSideSize * (map.GetLength(1) + 1));
			var areaMin  = -areaSize / 2;
			var areaRect = new Rect(areaMin, areaSize);
			starter.PlayerCameraFollower.Init(starter.MainCamera, starter.Player.transform, areaRect);
			BordersRoot.localScale = new Vector3(areaSize.x, areaSize.y, 1);
			Debug.Log($"Level generated. Seed {randomSeed}");
		}

		void ResetState() {
			_state = new LevelGeneratorState();
		}

		GameObject GetChunk(BaseMapCell baseMapCell) {
			switch ( baseMapCell.CellType ) {
				case MapCellType.SafeZone: {
					return Instantiate(Creator.SafeAreaPrefab);
				}
				case MapCellType.IdleEnemies: {
					return Creator.CreateRandomIdleChunk();
				}
				case MapCellType.Generator: {
					if ( !(baseMapCell is GeneratorMapCell generatorCell) ) {
						Debug.LogError("Can't instantiate generator cell - map cell class type is incorrect");
						return Creator.CreateEmptyChunk();
					}
					return Creator.CreateGeneratorChunk(generatorCell.GeneratorGridSize, PowerUpPlacesCountInGeneratorChunk);
				}
				case MapCellType.Empty: {
					return Creator.CreateEmptyChunk();
				}
				default: {
					Debug.LogError($"Unknown map cell type {baseMapCell.CellType}");
					return Creator.CreateEmptyChunk();
				}
			}
		}

		void UpdateMapSizesInState() {
			var idleEnemyChunkSize = (_levelInfo.EnemyGroupsCount > 0) ? LevelInfo.IdleEnemyGroupChunkSize : 0;
			_state.LevelChunkSideSize = Mathf.Max(_levelInfo.GeneratorsSideSize * LevelInfo.GeneratorCellSize + MinSpaceBetweenGenerators / 2 , idleEnemyChunkSize);
			// Calculating level size in chunks
			// Generating free cells count
			var freeCellsOnLevel = Random.Range(0, 6);
			// Calculating total cells count. +1 cell for safe zone rect
			var neededCellsTotal = _levelInfo.GeneratorsCount + _levelInfo.EnemyGroupsCount + 1 + freeCellsOnLevel;
			_state.LevelSideBlocksCount = Mathf.CeilToInt(Mathf.Sqrt(neededCellsTotal));
			// Making level side size odd for placing player's safe zone in (0,0)
			if ( _state.LevelSideBlocksCount % 2 == 0 ) {
				_state.LevelSideBlocksCount++;
			}
		}

		BaseMapCell[,] GenerateMap() {
			var map   = new BaseMapCell[_state.LevelSideBlocksCount, _state.LevelSideBlocksCount];
			// Register all cells
			var cells = new List<Vector2Int>();
			for ( var y = 0; y < _state.LevelSideBlocksCount; y++ ) {
				for ( var x = 0; x < _state.LevelSideBlocksCount; x++ ) {
					cells.Add(new Vector2Int(x, y));
				}
			}
			// Reserve one rect for safe area
			var safeRectCoords = new Vector2Int(_state.LevelSideBlocksCount / 2, _state.LevelSideBlocksCount / 2);
			cells.Remove(safeRectCoords);
			map[safeRectCoords.x, safeRectCoords.y] = new BaseMapCell{ CellType = MapCellType.SafeZone };
			// Creating generators
			var neededGenerators = _levelInfo.GeneratorsCount;
			while ( (neededGenerators > 0) && (cells.Count > 0) ) {
				var cellCoords = RandomUtils.GetAndRemoveRandomElement(cells);
				map[cellCoords.x, cellCoords.y] = new GeneratorMapCell(_levelInfo.GeneratorsSideSize);
				neededGenerators--;
			}
			// Creating idle groups
			var neededIdleGroups = _levelInfo.EnemyGroupsCount;
			while ( (neededIdleGroups > 0) && (cells.Count > 0) ) {
				var cellCoords = RandomUtils.GetAndRemoveRandomElement(cells);
				map[cellCoords.x, cellCoords.y] = new BaseMapCell{ CellType = MapCellType.IdleEnemies};
				neededIdleGroups--;
			}
			//Fill rest as empty cells
			foreach ( var cellCoords in cells ) {
				map[cellCoords.x, cellCoords.y] = new BaseMapCell{ CellType = MapCellType.Empty};
			}
			return map;
		}

		void AddPowerUpsToLevel(List<Transform> freePowerUpsSpawnPoints, PowerUpType powerUpType) {
			var powerUpSpawn = RandomUtils.GetAndRemoveRandomElement(freePowerUpsSpawnPoints);
			if ( !powerUpSpawn ) {
				Debug.LogError("Can't spawn power up - no available spawn points");
				return;
			}
			var prefab            = GetPowerUpPrefab(powerUpType);
			Instantiate(prefab, powerUpSpawn.position, Quaternion.identity, powerUpSpawn.parent);
			if ( _state.CreatedPowerUpsCount.ContainsKey(powerUpType) ) {
				_state.CreatedPowerUpsCount[powerUpType]++;
			} else {
				_state.CreatedPowerUpsCount[powerUpType] = 1;
			}
		}

		GameObject GetPowerUpPrefab(PowerUpType powerUpType) {
			return _prefabsController.GetPowerUpPrefab(powerUpType);
		}
	}
}