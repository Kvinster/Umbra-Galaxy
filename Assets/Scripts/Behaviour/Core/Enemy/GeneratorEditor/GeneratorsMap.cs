using UnityEngine;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	public class GeneratorsMap : Map<PlaceType>{
		public GeneratorsMap(int size) : base(size) { }

		public GeneratorsMap(Vector2Int size) : base(size) { }
	}
}