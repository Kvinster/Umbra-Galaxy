using UnityEngine;
using UnityEngine.UI;

using STP.Utils;

using NaughtyAttributes;
using STP.Behaviour.Core.Enemy.GeneratorEditor.RandomWalk;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	public class GeneratorCreator : GameComponent {
		public int  GridSize;
		public bool VisualizeMap;
		public bool VisualizeConvertedMap;

		[Header("For generator")]
		public GameObject GeneratorPrefab;

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

			Vector2 startPoint = GetStartPoint(map);
			var startGenGo     = Instantiate(GeneratorPrefab, startPoint * 100, Quaternion.identity, baseGo.transform);
			var startGenerator = startGenGo.GetComponent<Generator>();

			for ( var y = 0; y < map.Size; y++ ) {
				for ( var x = 0; x < map.Size; x++ ) {
					var cell  = map.GetCell(x, y);
					if ( cell == PlaceType.MainGenerator ) {
						continue;
					}
					if ( cell == PlaceType.SubGenerator ) {
						var genGo = Instantiate(GeneratorPrefab, new Vector3(x, y, 0) * 100, Quaternion.identity, baseGo.transform);
						var genComp = genGo.GetComponent<Generator>();
						startGenerator.SubGenerators.Add(genComp);
					}
				}
			}
		}

		Vector2Int GetStartPoint(GeneratorsMap map) {
			for ( var y = 0; y < map.Size; y++ ) {
				for ( var x = 0; x < map.Size; x++ ) {
					if ( map.GetCell(x, y) == PlaceType.MainGenerator ) {
						return new Vector2Int(x, y);
					}
				}
			}

			return -Vector2Int.one;
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


	}
}