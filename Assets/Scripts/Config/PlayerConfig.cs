using UnityEngine;
using UnityEngine.Assertions;

namespace STP.Config {
	[CreateAssetMenu(fileName = "PlayerConfig", menuName = "ScriptableObjects/PlayerConfig")]
	public sealed class PlayerConfig : ScriptableObject {
		const string Path = "PlayerConfig";

		static PlayerConfig _instance;

		public static PlayerConfig Instance {
			get {
				if ( !_instance ) {
					_instance = Resources.Load<PlayerConfig>(Path);
					Assert.IsTrue(_instance);
				}
				return _instance;
			}
		}

		public float MaxHp;
		public float MovementSpeed;
		public float FireRate;
		public float Damage;
	}
}
