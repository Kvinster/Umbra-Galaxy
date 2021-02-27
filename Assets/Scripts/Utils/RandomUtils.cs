using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using STP.Config;

namespace STP.Utils {
	public static class RandomUtils {
		public static T GetRandomElementOrDefault<T>(List<T> collection, T def) {
			if ( collection.Count == 0 ) {
				return def;
			}
			var index = Random.Range(0, collection.Count);
			return collection[index];
		}

		public static T GetRandomElement<T>(List<T> collection) {
			if ( collection.Count == 0 ) {
				Debug.LogWarning("Collection is empty. Returning default value");
				return default;
			}
			var index = Random.Range(0, collection.Count);
			return collection[index];
		}


		public static T GetAndRemoveRandomElement<T>(List<T> collection) {
			if ( collection.Count == 0 ) {
				Debug.LogWarning("Collection is empty. Returning default value");
				return default;
			}
			var index = Random.Range(0, collection.Count);
			var res   = collection[index];
			collection.RemoveAt(index);
			return res;
		}
	}
}