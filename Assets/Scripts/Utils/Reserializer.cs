#if UNITY_EDITOR

using UnityEditor;

using System.Linq;

namespace STP.Utils {
	public static class Reserializer {
		[MenuItem("Utils/Reserialize/Reserialize All Assets")]
		static void ReserializeAll() {
			var paths = AssetDatabase.GetAllAssetPaths();
			AssetDatabase.ForceReserializeAssets(paths);
		}
		
		[MenuItem("Utils/Reserialize/Reserialize Main Assets")]
		static void ReserializeMain() {
			var paths = AssetDatabase.GetAllAssetPaths()
				.Where(x => x.EndsWith(".prefab") || x.EndsWith(".asset") || x.EndsWith(".unity"));
			AssetDatabase.ForceReserializeAssets(paths);
		}

		[MenuItem("Utils/Reserialize/Reserialize Selected Assets")]
		static void ReserializeSelected() {
			var paths = Selection.objects.Select(AssetDatabase.GetAssetPath);
			AssetDatabase.ForceReserializeAssets(paths);
		}
	}
}

#endif