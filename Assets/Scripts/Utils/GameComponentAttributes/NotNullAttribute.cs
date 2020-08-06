using System;

namespace STP.Utils.GameComponentAttributes {
	[AttributeUsage(AttributeTargets.Field)]
	public class NotNullAttribute : BaseGameComponentAttribute {
		public NotNullAttribute() : this(true) { }

		public NotNullAttribute(bool checkPrefab = true) : base(checkPrefab) { }
	}
}