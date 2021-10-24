﻿using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Enemy.GeneratorEditor;
using STP.Config;
using STP.Core;
using STP.Utils;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core.Generators.Regular {
	public sealed class RegularLevelGeneratorImpl : ILevelGeneratorImpl {
		const int MinSpaceBetweenGenerators = 300;

		readonly RegularLevelGeneratorState _state = new RegularLevelGeneratorState();

		readonly RegularLevelInfo  _levelInfo;
		readonly Transform         _levelObjectsRoot;
		readonly Transform         _bordersRoot;
		readonly Player            _player;
		readonly PrefabsController _prefabsController;

		readonly ChunkCreator _chunkCreator;

		public Rect AreaRect { get; private set; }

		public RegularLevelGeneratorImpl(RegularLevelInfo levelInfo, Transform levelObjectsRoot, Transform bordersRoot,
			Player player, PrefabsController prefabsController) {
			_levelInfo            = levelInfo;
			_levelObjectsRoot     = levelObjectsRoot;
			_bordersRoot          = bordersRoot;
			_player               = player;
			_prefabsController    = prefabsController;

			_chunkCreator = new ChunkCreator();
		}

		public async UniTask GenerateLevel() {
			var randomSeed = Random.Range(int.MinValue, int.MaxValue);
			Random.InitState(randomSeed);
			UpdateMapSizesInState();
			var map            = GenerateMap();
			var levelSpaceSize = _state.LevelSideBlocksCount * _state.LevelChunkSideSize;
			var minPoint = new Vector2(-levelSpaceSize / 2.0f, -levelSpaceSize / 2.0f) +
			               new Vector2(_state.LevelChunkSideSize / 2.0f, _state.LevelChunkSideSize / 2.0f);
			// Create map chunks
			for ( var y = 0; y < map.GetLength(1); y++ ) {
				for ( var x = 0; x < map.GetLength(0); x++ ) {
					var mapCell = map[x, y];
					var obj     = GetChunk(mapCell);
					var pos     = (Vector3) minPoint + new Vector3(x * _state.LevelChunkSideSize, y * _state.LevelChunkSideSize);
					obj.transform.SetParent(_levelObjectsRoot);
					obj.transform.position = pos;
				}
				await UniTask.Yield();
			}
			// Init player camera follower
			var areaSize = new Vector2(_state.LevelChunkSideSize * (map.GetLength(0) + 1) , _state.LevelChunkSideSize * (map.GetLength(1) + 1));
			var areaMin  = -areaSize / 2;
			AreaRect = new Rect(areaMin, areaSize);
			_bordersRoot.localScale = new Vector3(areaSize.x, areaSize.y, 1);
			Debug.Log($"Level generated. Seed {randomSeed}");
		}

		GameObject GetChunk(BaseMapCell baseMapCell) {
			switch ( baseMapCell.CellType ) {
				case MapCellType.IdleEnemies: {
					return _chunkCreator.CreateRandomIdleChunk();
				}
				case MapCellType.Generator: {
					if ( !(baseMapCell is GeneratorMapCell generatorCell) ) {
						Debug.LogError("Can't instantiate generator cell - map cell class type is incorrect");
						return _chunkCreator.CreateEmptyChunk();
					}
					return _chunkCreator.CreateGeneratorChunk(generatorCell.GeneratorGridSize);
				}
				case MapCellType.Empty: {
					return _chunkCreator.CreateEmptyChunk();
				}
				default: {
					Debug.LogError($"Unknown map cell type {baseMapCell.CellType}");
					return _chunkCreator.CreateEmptyChunk();
				}
			}
		}

		void UpdateMapSizesInState() {
			var idleEnemyChunkSize = (_levelInfo.EnemyGroupsCount > 0) ? RegularLevelInfo.IdleEnemyGroupChunkSize : 0;
			_state.LevelChunkSideSize =
				Mathf.Max(
					_levelInfo.GeneratorsSideSize * RegularLevelInfo.GeneratorCellSize + MinSpaceBetweenGenerators / 2 ,
					idleEnemyChunkSize);
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
			map[safeRectCoords.x, safeRectCoords.y] = new BaseMapCell{ CellType = MapCellType.Empty };
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
	}
}
