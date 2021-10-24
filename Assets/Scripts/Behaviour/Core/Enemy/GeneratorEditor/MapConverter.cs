using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Enemy.GeneratorEditor.RandomWalk;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	public static class MapConverter {
		static List<Vector2Int> PossibleDirections => new List<Vector2Int> {
			Vector2Int.down,
			Vector2Int.left,
			Vector2Int.right,
			Vector2Int.up
		};

		public static GeneratorsMap ConvertMap(WalkMap map) {
			var res = new GeneratorsMap(map.Size);
			for ( var y = 0; y < map.Size.y; y++ ) {
				for ( var x = 0; x < map.Size.x; x++ ) {
					var cell = map.GetCell(x, y);
					if ( cell == CellState.StartPoint ) {
						res.SetCell(x, y, PlaceType.MainGenerator);
						continue;
					}
					if ( cell == CellState.Wall ) {
						var count = 0;
						foreach ( var direction in PossibleDirections ) {
							var newPoint = direction + new Vector2Int(x, y);
							var newCell  = map.GetCell(newPoint.x, newPoint.y);
							if ( (newCell == CellState.Wall) || (newCell == CellState.StartPoint) ) {
								count++;
							}
						}
						res.SetCell(x, y, count == 1 ? PlaceType.SubGenerator : PlaceType.Connector);
						continue;
					}
					res.SetCell(x, y, PlaceType.Nothing);
				}
			}
			return res;
		}
	}
}