using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;

namespace STP.Behaviour.Core {
	public sealed class CoreSpawnHelper {
		readonly CoreStarter _starter;

		readonly List<BaseCoreComponent> _components = new List<BaseCoreComponent>(5);

		public CoreSpawnHelper(CoreStarter starter) {
			_starter = starter;
		}

		public void TryInitSpawnedObject(GameObject gameObject) {
			if ( !gameObject ) {
				Debug.LogError("Game object is null");
				return;
			}
			_components.Clear();
			gameObject.GetComponentsInChildren(_components);
			foreach ( var comp in _components ) {
				comp.Init(_starter);
			}
			_components.Clear();
		}
	}
}
