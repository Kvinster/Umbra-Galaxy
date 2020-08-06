namespace STP.Utils.GameComponentAttributes {
	public sealed class CountAttribute : BaseGameComponentAttribute {
		public readonly int MinCount   = -1;
		public readonly int MaxCount   = -1;
		public readonly int ExactCount = -1;

		public CountAttribute(int minCount, int maxCount, bool checkPrefab = true) : base(checkPrefab) {
			MinCount = minCount;
			MaxCount = maxCount;
		}

		public CountAttribute(int exactCount, bool checkPrefab = true) : base(checkPrefab) {
			ExactCount = exactCount;
		}
	}
}