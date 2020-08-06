namespace STP.Utils.GameComponentAttributes {
	public class EnumNotEqualsAttribute : BaseGameComponentAttribute {
		public readonly int EnumIntValue;
		
		public EnumNotEqualsAttribute(int enumIntValue, bool checkPrefab = true) : base(checkPrefab) {
			EnumIntValue = enumIntValue;
		}
	}
}