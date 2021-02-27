namespace STP.Behaviour.Core.Generators {
	public class GeneratorMapCell : BaseMapCell {
		public int GeneratorGridSize;

		public GeneratorMapCell(int gridSize) {
			CellType          = MapCellType.Generator;
			GeneratorGridSize = gridSize;
		}
	}
}