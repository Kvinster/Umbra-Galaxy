using UnityEngine;

namespace STP.Config {
	public abstract class BaseLevelInfo : ScriptableObject {
		public abstract LevelType LevelType { get; }
	}
}
