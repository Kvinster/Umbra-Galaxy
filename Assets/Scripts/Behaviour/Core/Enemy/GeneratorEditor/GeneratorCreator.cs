#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using STP.Behaviour.Core.Enemy.GeneratorEditor.RandomWalk;
using STP.Utils;

using NaughtyAttributes;
using Shapes;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	public class GeneratorCreator : GameComponent {
		#if UNITY_EDITOR
		public int  GridSize;
		public bool VisualizeMap;
		public bool VisualizeConvertedMap;

		[Header("For generator")]
		public GameObject MainGeneratorPrefab;
		public GameObject GeneratorPrefab;
		public GameObject ConnectorPrefab;
		public GameObject LinePrefab;

		List<Vector2Int> PossibleDirections => new List<Vector2Int> {
			Vector2Int.down,
			Vector2Int.left,
			Vector2Int.up,
			Vector2Int.right
		};

		Vector2Int InvalidVector => -Vector2Int.one;

		[Button("Create generator")]
		void CreateGenerator() {
			var mazeGen = new RandomWalkMapGenerator();
			var cellMap = mazeGen.CreateMaze(GridSize);
			var map     = MapConverter.ConvertMap(cellMap);
			if ( VisualizeMap ) {
				VisualizeMaze(cellMap);
			}
			if ( VisualizeConvertedMap ) {
				VisualizeMaze(map);
			}
			CreateGenerators(map);
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

		void CreateGenerators(GeneratorsMap map) {
			var baseGo = new GameObject();
			baseGo.transform.SetParent(gameObject.transform);

			var connectorsMap = new Map<Connector>(map.Size);
			var mainGenPoint  = InvalidVector;

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
						genGo.transform.position = new Vector3(x, y, 0) * 100;
						var genComp = genGo.GetComponent<Generator>();
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
						connectorGo.transform.position = new Vector3(x, y, 0) * 100;
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
				VisitPoint(startLink);
				CreateLine(mainConnector, startLink);
			}

		}

		void CrawlConnectors(Map<Connector> connectorMap, Vector2Int curPoint) {
			var curConnector = connectorMap.GetCell(curPoint.x, curPoint.y);
			foreach ( var dir in PossibleDirections ) {
				var newPoint  = curPoint + dir;
				var connector = connectorMap.GetCell(newPoint.x, newPoint.y);
				if ( !connector ) {
					continue;
				}
				if ( connector.Children.Contains(curConnector) ) {
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
			var lineGo   = Instantiate(LinePrefab, one.transform);
			var lineComp = lineGo.GetComponent<Line>();
			lineComp.Start = Vector3.zero;
			lineComp.End   = other.transform.position - one.transform.position;
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