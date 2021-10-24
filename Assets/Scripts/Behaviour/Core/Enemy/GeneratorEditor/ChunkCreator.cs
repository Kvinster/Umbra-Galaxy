using UnityEngine;
using UnityEngine.Assertions;

using System.Collections.Generic;

using STP.Behaviour.Core.Generators;
using STP.Behaviour.Core.Enemy.GeneratorEditor.RandomWalk;
using STP.Utils;

using Shapes;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	public sealed class ChunkCreator {
		static List<Vector2Int> PossibleDirections => new List<Vector2Int> {
			Vector2Int.down,
			Vector2Int.left,
			Vector2Int.up,
			Vector2Int.right
		};

		static Vector2Int InvalidVector => -Vector2Int.one;

		readonly ChunkConfig _config;

		public ChunkCreator() {
			_config = Resources.Load<ChunkConfig>("ChunkConfig");
			Assert.IsTrue(_config);
		}

		public GameObject CreateEmptyChunk() {
			return CreateGeneratorChunk(0);
		}

		public GameObject CreateGeneratorChunk(int gridSize) {
			return CreateGeneratorChunk(new Vector2Int(gridSize, gridSize));
		}

		public GameObject CreateGeneratorChunk(Vector2Int gridSize) {
			var mazeGen = new RandomWalkMapGenerator();
			var cellMap = mazeGen.CreateMaze(gridSize);
			var map     = MapConverter.ConvertMap(cellMap);
			var go      = CreateGeneratorsVariant(map);
			return go;
		}

		public GameObject CreateRandomIdleChunk() {
			return Object.Instantiate(RandomUtils.GetRandomElement(_config.IdleChunks));
		}

		GameObject CreateGeneratorsVariant(GeneratorsMap map) {
			var cellSize = 100;
			var baseGo   = new GameObject($"generator_{map.Size}");
			if ( map.Size == Vector2Int.zero ) {
				return baseGo;
			}
			// Create generators and init connectors map
			var connectorsMap = new Map<Connector>(map.Size);
			var mainGenPoint  = InvalidVector;
			for ( var y = 0; y < map.Size.y; y++ ) {
				for ( var x = 0; x < map.Size.x; x++ ) {
					var cell     = map.GetCell(x, y);
					var worldPos = new Vector3(x - map.Size.x / 2, y - map.Size.y / 2, 0);
					if ( cell == PlaceType.MainGenerator ) {
						mainGenPoint = new Vector2Int(x, y);
					}

					if ( cell == PlaceType.MainGenerator || cell == PlaceType.SubGenerator ) {
						var genPrefab = (cell == PlaceType.MainGenerator)
							? _config.MainGeneratorPrefab
							: _config.GeneratorPrefab;
						var genGo = Object.Instantiate(genPrefab, baseGo.transform);
						if ( !genGo ) {
							Debug.LogError("Can't cast instance to GameObject. aborting instancing generator");
							continue;
						}
						genGo.transform.position = worldPos * cellSize;
						var genComp    = genGo.GetComponent<Generator>();
						var bulletPair = RandomUtils.GetRandomElement(_config.BulletPrefabs);
						genComp.ShootingParams.BulletPrefab    = (cell == PlaceType.MainGenerator) ?  bulletPair.MainGenBullet : bulletPair.SubGenBullet;
						genComp.IsMainGenerator = (cell == PlaceType.MainGenerator);
						var connector = genGo.GetComponentInChildren<Connector>();
						connectorsMap.SetCell(x, y, connector);
					}

					if ( cell == PlaceType.Connector ) {
						var connectorGo = Object.Instantiate(_config.ConnectorPrefab, baseGo.transform);
						if ( !connectorGo ) {
							Debug.LogError("Can't cast instance to GameObject. aborting instancing connector");
							continue;
						}
						connectorGo.transform.position = worldPos * cellSize;
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
			var lineGo               = Object.Instantiate(_config.LinePrefab, one.transform);
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
			var size = new Vector2(
				Mathf.Abs(!Mathf.Approximately(vectorToConnectorEnd.x, 0) ? vectorToConnectorEnd.x : 0) +
				lineComp.Thickness,
				Mathf.Abs(!Mathf.Approximately(vectorToConnectorEnd.y, 0) ? vectorToConnectorEnd.y : 0) +
				lineComp.Thickness);
			collider.size = size;
		}
	}
}