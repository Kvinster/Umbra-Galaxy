using UnityEngine;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor.RandomWalk {
	public class WalkMap : Map<CellState> {
		public WalkMap(int size) : base(size) { }

		public WalkMap(Vector2Int size) : base(size) { }
	}
}