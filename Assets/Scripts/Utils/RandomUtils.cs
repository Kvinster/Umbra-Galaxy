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

		public static T GetRandomWeightedElement<T>(List<T> collection) where T : WeightedValue {
			var totalWeight = collection.Sum(chunk => chunk.Weight);
			var randomValue = Random.Range(0, totalWeight);
			foreach ( var obj in collection ) {
				if ( obj.Weight > randomValue ) {
					return obj;
				}
				randomValue -= obj.Weight;
			}
			Debug.LogWarning("Something went wrong. Returning default value");
			return default;
		}
	}
}