using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Core.Generators;
using STP.Behaviour.Core.Enemy.GeneratorEditor.RandomWalk;
using STP.Utils;

using Shapes;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	public class ChunkCreator : GameComponent {
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

		[Header("Specific chunks")]
		public GameObject SafeAreaPrefab;

		List<Vector2Int> PossibleDirections => new List<Vector2Int> {
			Vector2Int.down,
			Vector2Int.left,
			Vector2Int.up,
			Vector2Int.right
		};

		Vector2Int InvalidVector => -Vector2Int.one;

		public GameObject CreateGeneratorChunk(int gridSize, int powerUpPointsCount) {
			var mazeGen = new RandomWalkMapGenerator();
			var cellMap = mazeGen.CreateMaze(gridSize);
			var map     = MapConverter.ConvertMap(cellMap);
			var go      = CreateGeneratorsVariant(map, powerUpPointsCount);
			return go;
		}

		GameObject CreateGeneratorsVariant(GeneratorsMap map, int powerUpPointsCount) {
			var cellSize = 100;

			var baseGo = new GameObject();

			var comp = baseGo.AddComponent<LevelChunk>();

			// Create Random powerup points
			for ( var i = 0; i < powerUpPointsCount; i++ ) {
				var x     = Random.Range(-map.Size / 2f, map.Size / 2f) * cellSize;
				var y     = Random.Range(-map.Size / 2f, map.Size / 2f) * cellSize;
				var point = new GameObject();
				point.transform.position = new Vector3(x, y, 0);
				point.transform.SetParent(baseGo.transform);
				comp.FreePowerUpSpawnPoints.Add(point.transform);
			}

			if ( map.Size == 0 ) {
				return baseGo;
			}

			var connectorsMap = new Map<Connector>(map.Size);
			var mainGenPoint  = InvalidVector;
			// Create generators and init connectors map
			for ( var y = 0; y < map.Size; y++ ) {
				for ( var x = 0; x < map.Size; x++ ) {
					var cell     = map.GetCell(x, y);
					var worldPos = new Vector3(x - map.Size / 2, y - map.Size / 2, 0);
					if ( cell == PlaceType.MainGenerator ) {
						mainGenPoint = new Vector2Int(x, y);
					}

					if ( cell == PlaceType.MainGenerator || cell == PlaceType.SubGenerator ) {
						var genPrefab = (cell == PlaceType.MainGenerator) ? MainGeneratorPrefab : GeneratorPrefab;
						var genGo = Instantiate(genPrefab, baseGo.transform);
						if ( !genGo ) {
							Debug.LogError("Can't cast instance to GameObject. aborting instancing generator");
							continue;
						}
						genGo.transform.position = worldPos * cellSize;
						var genComp    = genGo.GetComponent<Generator>();
						var bulletPair = RandomUtils.GetRandomElement(BulletPrefabs);
						genComp.BulletPrefab    = (cell == PlaceType.MainGenerator) ?  bulletPair.MainGenBullet : bulletPair.SubGenBullet;
						genComp.IsMainGenerator = (cell == PlaceType.MainGenerator);
						var connector = genGo.GetComponentInChildren<Connector>();
						connectorsMap.SetCell(x, y, connector);
					}

					if ( cell == PlaceType.Connector ) {
						var connectorGo = Instantiate(ConnectorPrefab, baseGo.transform);
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
			var lineGo               = Instantiate(LinePrefab, one.transform);
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
	}
}