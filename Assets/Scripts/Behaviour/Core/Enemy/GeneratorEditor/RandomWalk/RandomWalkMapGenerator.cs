using UnityEngine;

using System.Collections.Generic;

using STP.Utils;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor.RandomWalk {
	public class RandomWalkMapGenerator {
		List<Vector2Int> StartHorizontalDirections => new List<Vector2Int> {
			Vector2Int.left,
			Vector2Int.right
		};

		List<Vector2Int> StartVerticalDirections => new List<Vector2Int> {
			Vector2Int.up,
			Vector2Int.down
		};

		List<Vector2Int> PossibleWalkDirections => new List<Vector2Int> {
			Vector2Int.left,
			Vector2Int.right,
			Vector2Int.up,
			Vector2Int.down
		};

		Vector2Int InvalidVector => new Vector2Int(-1, -1);

		public WalkMap CreateMaze(int size) {
			var map = new WalkMap(size);

			if ( size == 0 ) {
				return map;
			}

			// Prepare map
			for ( var y = 0; y < size; y++ ) {
				for ( var x = 0; x < size; x++ ) {
					map.SetCell(x, y, CellState.Empty);
				}
			}

			var startPoint  = new Vector2Int(size / 2, size / 2);
			var startDirs   = Random.Range(0, 2) == 0 ? StartHorizontalDirections : StartVerticalDirections;
			var pointsList = new List<Vector2Int>();

			foreach ( var dir in startDirs ) {
				pointsList.Add(dir + startPoint);
			}

			while ( pointsList.Count > 0 ) {
				var curPoint = RandomUtils.GetAndRemoveRandomElement(pointsList);
				map.SetCell(curPoint.x, curPoint.y, CellState.Wall);
				var dir = GetRandomDir(map, curPoint);
				if ( dir == InvalidVector ) {
					continue;
				}
				var newPoint  = curPoint + dir;
				var newPoint2 = curPoint + 2 * dir;
				map.SetCell(newPoint.x, newPoint.y, CellState.Wall);
				map.SetCell(newPoint2.x, newPoint2.y, CellState.Wall);
				pointsList.Add(newPoint2);
				pointsList.Add(curPoint);
			}

			map.SetCell(startPoint.x, startPoint.y, CellState.StartPoint);
			return map;
		}

		Vector2Int GetRandomDir(WalkMap map, Vector2Int point) {
			var allDirs = PossibleWalkDirections;
			allDirs.RemoveAll((dir) => {
				var newPoint = point + 2 * dir;
				var cell     = map.GetCell(newPoint.x, newPoint.y);
				return (cell == CellState.Wall) || (cell == CellState.None);
			});
			return RandomUtils.GetRandomElementOrDefault(allDirs, InvalidVector);
		}
	}
}