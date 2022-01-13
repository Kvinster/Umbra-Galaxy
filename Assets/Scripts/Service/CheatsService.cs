using UnityEngine;

namespace STP.Service {
	public static class CheatsService {
		public static bool PlayerInvincible;
		public static bool ShowCheatPanel;

		sealed class CheatsContext : MonoBehaviour {
			void Update() {
				if ( Input.GetKey(KeyCode.LeftShift) ) {
					if ( Input.GetKeyDown(KeyCode.Q)) {
						PlayerInvincible = !PlayerInvincible;
					}
					if ( Input.GetKeyDown(KeyCode.W) ) {
						ShowCheatPanel = !ShowCheatPanel;
					}
					if ( Input.GetKeyDown(KeyCode.Alpha1) ) {
						Time.timeScale = 0f;
					} else if ( Input.GetKeyDown(KeyCode.Alpha2) ) {
						Time.timeScale = 0.25f;
					} else if ( Input.GetKeyDown(KeyCode.Alpha3) ) {
						Time.timeScale = 0.5f;
					} else if ( Input.GetKeyDown(KeyCode.Alpha4) ) {
						Time.timeScale = 1f;
					}
				}
				
			}
		}
		
		[RuntimeInitializeOnLoadMethod]
		static void Initialize() {
			var go = new GameObject("[CheatsContext]");
			go.AddComponent<CheatsContext>();
			Object.DontDestroyOnLoad(go);
		}
	}
}