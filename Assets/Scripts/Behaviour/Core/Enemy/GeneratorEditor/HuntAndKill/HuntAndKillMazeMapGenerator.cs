using UnityEngine;

using System.Collections.Generic;

using STP.Utils;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	public class HuntAndKillMazeMapGenerator {
		public MazeMap CreateMaze(int size) {
			var map = new MazeMap(size);
			// Create walls
			for ( var y = 0; y < size; y++ ) {
				for ( var x = 0; x < size; x++ ) {
					if ( (y == 0) || (x == 0) || (x == size - 1) || (y == size - 1) ) {
						map.SetCell(x, y, CellState.Wall);
						continue;
					}
					if ( (y % 2 == 0) || (x % 2 == 0) ) {
						map.SetCell(x, y, CellState.Wall);
					} else {
						map.SetCell(x, y, CellState.NotVisited);
					}
				}
			}

			// Select random start point
			var start  = new Vector2Int(size / 2 + size % 2, size / 2 + size % 2);
			map.SetCell(start.x, start.y, CellState.Visited);

			var curPoint = start;
			// Creating maze
			while ( HaveNotVisitedCellOnMap(map) ) {
				// Random walk
				while ( HaveNotVisitedNeighbour(map, curPoint) ) {
					var wallPoint      = GetRandomNeighbourWall(map, curPoint);
					var neighbourPoint = (wallPoint - curPoint) * 2 + curPoint;
					map.SetCell(wallPoint.x     , wallPoint.y     , CellState.DestroyedWall);
					map.SetCell(neighbourPoint.x, neighbourPoint.y, CellState.Visited);
					curPoint = neighbourPoint;

				}
				var newPoint     = Scan(map);
				if ( newPoint == -Vector2Int.one ) {
					break;
				}
				var visitedPoint = GetSpecificNeighbour(map, newPoint, CellState.Visited);
				var wall         = (newPoint - visitedPoint) / 2 + visitedPoint;
				map.SetCell(wall.x    , wall.y    , CellState.DestroyedWall);
				map.SetCell(newPoint.x, newPoint.y, CellState.Visited);
				curPoint = newPoint;
			}

			map.SetCell(start.x, start.y, CellState.StartPoint);
			return map;
		}

		bool HaveNotVisitedCellOnMap(MazeMap map) {
			for ( var y = 1; y < map.Size; y += 2 ) {
				for ( var x = 1; x < map.Size; x += 2 ) {
					if ( map.GetCell(x, y) == CellState.NotVisited ) {
						return true;
					}
				}
			}
			return false;
		}

		Vector2Int GetRandomNeighbourWall(MazeMap map, Vector2Int point) {
			var possibleNeighboursDirection = new List<Vector2Int> {
				Vector2Int.left ,
				Vector2Int.right,
				Vector2Int.up,
				Vector2Int.down
			};

			possibleNeighboursDirection.RemoveAll(dir => {
				var wallPos      = point + dir;
				var neighbourPos = point + dir * 2;

				var wallExists      = map.GetCell(wallPos.x, wallPos.y) == CellState.Wall;
				var neighbourExists = map.GetCell(neighbourPos.x, neighbourPos.y) == CellState.NotVisited;

				return !neighbourExists || !wallExists;
			});
			if ( possibleNeighboursDirection.Count == 0 ) {
				return -Vector2Int.one;
			}
			return RandomUtils.GetAndRemoveRandomElement(possibleNeighboursDirection) + point;
		}

		Vector2Int Scan(MazeMap map) {
			for ( var y = 1; y < map.Size; y += 2 ) {
				for ( var x = 1; x < map.Size; x += 2 ) {
					var point = new Vector2Int(x, y);
					if ( map.GetCell(x, y) == CellState.NotVisited && HaveSpecificNeighbour(map, point, CellState.Visited) ) {
						return point;
					}
				}
			}
			return -Vector2Int.one;
		}

		Vector2Int GetSpecificNeighbour(MazeMap map, Vector2Int point, CellState cellState) {
			var neighboursWallList = new List<Vector2Int> {
				point + Vector2Int.left * 2,
				point + Vector2Int.right * 2,
				point + Vector2Int.up * 2,
				point + Vector2Int.down * 2
			};
			foreach ( var neighbourPoint in neighboursWallList ) {
				if ( map.GetCell(neighbourPoint.x, neighbourPoint.y) == cellState ) {
					return neighbourPoint;
				}
			}
			return -Vector2Int.one;
		}

		bool HaveNotVisitedNeighbour(MazeMap map, Vector2Int point) {
			return HaveSpecificNeighbour(map, point, CellState.NotVisited);
		}

		bool HaveSpecificNeighbour(MazeMap map, Vector2Int point, CellState cellState) {
			var neighboursWallList = new List<Vector2Int> {
				point + Vector2Int.left * 2,
				point + Vector2Int.right * 2,
				point + Vector2Int.up * 2,
				point + Vector2Int.down * 2
			};
			foreach ( var neighbourPoint in neighboursWallList ) {
				if ( map.GetCell(neighbourPoint.x, neighbourPoint.y) == cellState ) {
					return true;
				}
			}
			return false;
		}
	}
}