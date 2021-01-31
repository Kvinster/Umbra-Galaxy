#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Collections.Generic;

using STP.Behaviour.Core.Generators;
using STP.Behaviour.Core.Enemy.GeneratorEditor.RandomWalk;
using STP.Config;
using STP.Utils;

using NaughtyAttributes;
using Shapes;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	public class GeneratorCreator : GameComponent {
		#if UNITY_EDITOR
		const string GeneratorsBasePathFormat = "Assets/Prefabs/LevelChunks/Generators/Size{0}/";
		const string GeneratorPrefabPrefix    = "Generator_";
		const string GeneratorPrefabSuffix    = ".prefab";
		const string GeneratorNameFormat      = GeneratorPrefabPrefix + "{0}" + GeneratorPrefabSuffix;

		public int  GridSize;
		public int  PowerUpPointsCount;
		public bool VisualizeMap;
		public bool VisualizeConvertedMap;

		[Serializable]
		public class BulletPair {
			public GameObject MainGenBullet;
			public GameObject SubGenBullet;
		}

		[Header("For generator")]
		public GameObject       MainGeneratorPrefab;
		public GameObject       GeneratorPrefab;
		public List<BulletPair> BulletPrefabs;
		public GameObject       ConnectorPrefab;
		public GameObject       LinePrefab;

		List<Vector2Int> PossibleDirections => new List<Vector2Int> {
			Vector2Int.down,
			Vector2Int.left,
			Vector2Int.up,
			Vector2Int.right
		};

		Vector2Int InvalidVector => -Vector2Int.one;

		[Button("Refresh created generators")]
		void RefreshCreatedGenerators() {
			var chunkConfig = Resources.Load<ChunkConfig>(ChunkConfig.ChunkConfigPath);
			foreach ( var chunkInfo in chunkConfig.ChunkInfos ) {
				if ( !chunkInfo.Prefab ) {
					continue;
				}
				var generatorChunkComp = chunkInfo.Prefab.GetComponent<LevelGeneratorChunk>();
				if ( !generatorChunkComp ) {
					continue;
				}
				var fullPath = AssetDatabase.GetAssetPath(chunkInfo.Prefab);
				File.Delete(fullPath);
				CreateGeneratorChunk(generatorChunkComp.Seed, generatorChunkComp.ChunkSize, PowerUpPointsCount);
			}
			chunkConfig.RegenerateConfig();
		}

		[Button("Create generator")]
		void CreateRandomGeneratorChunk() {
			var seed = Random.Range(int.MinValue, int.MaxValue);
			CreateGeneratorChunk(seed, GridSize, PowerUpPointsCount);
		}

		void CreateGeneratorChunk(int seed, int gridSize, int powerUpPointsCount) {
			Random.InitState(seed);

			var mazeGen = new RandomWalkMapGenerator();
			var cellMap = mazeGen.CreateMaze(gridSize);
			var map     = MapConverter.ConvertMap(cellMap);
			if ( VisualizeMap ) {
				VisualizeMaze(cellMap);
			}
			if ( VisualizeConvertedMap ) {
				VisualizeMaze(map);
			}
			var go = CreateGeneratorsVariant(map, seed, powerUpPointsCount);

			Directory.CreateDirectory(string.Format(GeneratorsBasePathFormat, gridSize));
			PrefabUtility.SaveAsPrefabAsset(go, string.Format(GeneratorsBasePathFormat, gridSize) + string.Format(GeneratorNameFormat, seed));

			DestroyImmediate(go);
		}

		void VisualizeMaze(WalkMap map) {
			var canvasGo = new GameObject();
			canvasGo.AddComponent<Canvas>();
			canvasGo.transform.SetParent(gameObject.transform);
			for ( var y = 0; y < map.Size; y++ ) {
				for ( var x = 0; x < map.Size; x++ ) {
					var go    = new GameObject();
					var image = go.AddComponent<Image>();
					image.color           = GetColor(map.GetCell(x, y));
					go.transform.position = new Vector3(x, y, 0) * 100;
					go.transform.SetParent(canvasGo.transform);
				}
			}
		}

		void VisualizeMaze(MazeMap map) {
			var canvasGo = new GameObject();
			canvasGo.AddComponent<Canvas>();
			canvasGo.transform.SetParent(gameObject.transform);
			for ( var y = 0; y < map.Size; y++ ) {
				for ( var x = 0; x < map.Size; x++ ) {
					var go    = new GameObject();
					var image = go.AddComponent<Image>();
					image.color           = GetColor(map.GetCell(x, y));
					go.transform.position = new Vector3(x, y, 0) * 100;
					go.transform.SetParent(canvasGo.transform);
				}
			}
		}

		void VisualizeMaze(GeneratorsMap map) {
			var canvasGo = new GameObject();
			canvasGo.AddComponent<Canvas>();
			canvasGo.transform.SetParent(gameObject.transform);
			for ( var y = 0; y < map.Size; y++ ) {
				for ( var x = 0; x < map.Size; x++ ) {
					var go    = new GameObject();
					var image = go.AddComponent<Image>();
					image.color = GetColor(map.GetCell(x, y));
					go.transform.position = new Vector3(x, y, 0) * 100;
					go.transform.SetParent(canvasGo.transform);
				}
			}
		}

		GameObject CreateGeneratorsVariant(GeneratorsMap map, int seed, int powerUpPointsCount) {

			var baseGo = new GameObject();

			var connectorsMap = new Map<Connector>(map.Size);
			var mainGenPoint  = InvalidVector;

			var cellSize = 100;

			// Create generators and init connectors map
			for ( var y = 0; y < map.Size; y++ ) {
				for ( var x = 0; x < map.Size; x++ ) {
					var cell  = map.GetCell(x, y);
					if ( cell == PlaceType.MainGenerator ) {
						mainGenPoint = new Vector2Int(x, y);
					}

					if ( cell == PlaceType.MainGenerator || cell == PlaceType.SubGenerator ) {
						var genPrefab = (cell == PlaceType.MainGenerator) ? MainGeneratorPrefab : GeneratorPrefab;
						var genGo = PrefabUtility.InstantiatePrefab(genPrefab, baseGo.transform) as GameObject;
						if ( !genGo ) {
							Debug.LogError("Can't cast instance to GameObject. aborting instancing generator");
							continue;
						}
						genGo.transform.position = new Vector3(x, y, 0) * cellSize;
						var genComp    = genGo.GetComponent<Generator>();
						var bulletPair = RandomUtils.GetRandomElement(BulletPrefabs);
						genComp.BulletPrefab    = (cell == PlaceType.MainGenerator) ?  bulletPair.MainGenBullet : bulletPair.SubGenBullet;
						genComp.IsMainGenerator = (cell == PlaceType.MainGenerator);
						var connector = genGo.GetComponentInChildren<Connector>();
						connectorsMap.SetCell(x, y, connector);
					}

					if ( cell == PlaceType.Connector ) {
						var connectorGo = PrefabUtility.InstantiatePrefab(ConnectorPrefab, baseGo.transform) as GameObject;
						if ( !connectorGo ) {
							Debug.LogError("Can't cast instance to GameObject. aborting instancing connector");
							continue;
						}
						connectorGo.transform.position = new Vector3(x, y, 0) * cellSize;
						var connector = connectorGo.GetComponent<Connector>();
						connectorsMap.SetCell(x, y, connector);
					}
				}
			}

			// Init connectors links
			var mainConnector = connectorsMap.GetCell(mainGenPoint.x, mainGenPoint.y);

			CrawlConnectors(connectorsMap, mainGenPoint);

			// Create connectors view
			foreach ( var startLink in mainConnector.Children ) {
				CreateLine(startLink, mainConnector);
				VisitPoint(startLink);
			}


			var comp       = baseGo.AddComponent<LevelGeneratorChunk>();
			comp.Seed      = seed;
			comp.ChunkSize = map.Size;

			// Create Random powerup points
			for ( var i = 0; i < powerUpPointsCount; i++ ) {
				var x = Random.Range(-map.Size / 2f, map.Size / 2f) * cellSize;
				var y = Random.Range(-map.Size / 2f, map.Size / 2f) * cellSize;
				var point = new GameObject();
				point.transform.position = new Vector3(x, y, 0);
				point.transform.SetParent(baseGo.transform);
				comp.FreePowerUpSpawnPoints.Add(point.transform);
			}

			return baseGo;
		}

		void CrawlConnectors(Map<Connector> connectorMap, Vector2Int curPoint) {
			var curConnector = connectorMap.GetCell(curPoint.x, curPoint.y);
			foreach ( var dir in PossibleDirections ) {
				var newPoint  = curPoint + dir;
				var connector = connectorMap.GetCell(newPoint.x, newPoint.y);
				if ( !connector ) {
					continue;
				}
				if ( connector.Children.Contains(curConnector) || (connector.Parent == curConnector) ) {
					continue;
				}
				connector.Parent = curConnector;
				curConnector.Children.Add(connector);
				CrawlConnectors(connectorMap, newPoint);
			}
		}

		void VisitPoint(Connector point) {
			var connectors = point.Children;
			foreach ( var connector in connectors ) {
				CreateLine(connector, point);
				VisitPoint(connector);
			}
		}

		void CreateLine(Connector one, Connector other) {
			var lineGo               = PrefabUtility.InstantiatePrefab(LinePrefab, one.transform) as GameObject;
			var lineComp             = lineGo.GetComponent<Line>();
			var vectorToConnectorEnd = (other.transform.position - one.transform.position);
			lineComp.Start = Vector3.zero;
			lineComp.End   = vectorToConnectorEnd;

			var collider = lineGo.GetComponent<BoxCollider2D>();
			if ( !collider ) {
				Debug.LogError("Can't init collider - collider not found");
				return;
			}

			collider.offset = vectorToConnectorEnd / 2;
			var size = new Vector2(Mathf.Abs(!Mathf.Approximately(vectorToConnectorEnd.x, 0) ? vectorToConnectorEnd.x : 0) + lineComp.Thickness, Mathf.Abs(!Mathf.Approximately(vectorToConnectorEnd.y, 0) ? vectorToConnectorEnd.y : 0)+ lineComp.Thickness);
			collider.size = size;
		}

		Color GetColor(PlaceType state) {
			switch ( state ) {
				case PlaceType.Nothing:
					return Color.clear;
				case PlaceType.Connector:
					return Color.blue;
				case PlaceType.SubGenerator:
					return Color.magenta;
				case PlaceType.MainGenerator:
					return Color.green;
			}
			return Color.clear;
		}

		Color GetColor(CellState state) {
			switch ( state ) {
				case CellState.Wall:
					return Color.grey;
				case CellState.None:
					return Color.clear;
				case CellState.Visited:
				case CellState.DestroyedWall:
					return Color.white;
				case CellState.NotVisited:
					return Color.black;
				case CellState.StartPoint:
					return Color.green;
			}
			return Color.clear;
		}

		Color GetColor(RandomWalk.CellState state) {
			switch ( state ) {
				case RandomWalk.CellState.Wall:
					return Color.grey;
				case RandomWalk.CellState.None:
					return Color.clear;
				case RandomWalk.CellState.Empty:
					return Color.white;
				case RandomWalk.CellState.StartPoint:
					return Color.green;
			}
			return Color.clear;
		}

		#endif
	}
}