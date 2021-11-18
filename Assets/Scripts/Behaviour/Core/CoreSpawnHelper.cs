using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;

namespace STP.Behaviour.Core {
	public sealed class CoreSpawnHelper {
		public readonly Transform TempObjRoot;
		readonly CoreStarter _starter;

		readonly List<BaseCoreComponent> _components = new List<BaseCoreComponent>(5);

		public CoreSpawnHelper(CoreStarter starter, Transform tempObjRoot) {
			_starter    = starter;
			TempObjRoot = tempObjRoot;
		}

		public void TryInitSpawnedObject(GameObject gameObject, bool isTempObject = true) {
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
			if ( isTempObject ) {
				gameObject.transform.SetParent(TempObjRoot);
			}
		}
	}
}
