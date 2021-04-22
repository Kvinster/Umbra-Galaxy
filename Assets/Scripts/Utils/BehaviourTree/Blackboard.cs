using UnityEngine;
using UnityEngine.Assertions;

using System.Collections.Generic;

namespace STP.Utils.BehaviourTree {
	public sealed class Blackboard {
		readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

		public void SetParameter(string key, object value) {
			_parameters[key] = value;
		}

		public void UnsetParameter(string key) {
			_parameters.Remove(key);
		}

		public T GetParameterOrDefault<T>(string key, T defaultValue = default) {
			return _parameters.ContainsKey(key) ? GetParameter<T>(key) : defaultValue;
		}

		public T GetParameter<T>(string key) {
			Assert.IsTrue(_parameters.ContainsKey(key));
			var valueRaw = _parameters[key];
			switch ( valueRaw ) {
				case null: {
					return default;
				}
				case T value: {
					return value;
				}
				default: {
					Debug.LogErrorFormat("{0}.{1}: invalid value type. Expected: '{2}', actual: '{3}'",
						nameof(Blackboard),
						nameof(GetParameter), typeof(T).Name, valueRaw?.GetType().Name);
					return default;
				}
			}
		}
	}
}
