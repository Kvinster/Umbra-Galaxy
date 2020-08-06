using System;

namespace STP.Utils.GameComponentAttributes {
	public abstract class BaseGameComponentAttribute : Attribute {
		public readonly bool CheckPrefab;

		protected BaseGameComponentAttribute(bool checkPrefab) {
			CheckPrefab = checkPrefab;
		}
	}
}