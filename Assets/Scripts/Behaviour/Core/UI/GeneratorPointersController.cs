using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;

namespace STP.Behaviour.Core.UI {
	public class GeneratorPointersController : BaseCoreComponent {
		public List<Pointer> Pointers;

		void OnDestroy() {
			GeneratorsWatcher.GeneratorsCountChanged -= OnGeneratorsCountChanged;
		}

		protected override void InitInternal(CoreStarter starter) {
			GeneratorsWatcher.GeneratorsCountChanged += OnGeneratorsCountChanged;
			OnGeneratorsCountChanged();
		}

		void OnGeneratorsCountChanged() {
			var generators = GeneratorsWatcher.MainGenerators;
			var minCount   = Mathf.Min(generators.Count, Pointers.Count);
			var i = 0;
			for ( ; i < minCount; i++ ) {
				var pointer = Pointers[i];
				pointer.Init(generators[i].transform);
				pointer.gameObject.SetActive(true);
			}
			for ( ; i < Pointers.Count; i++ ) {
				var pointer = Pointers[i];
				pointer.Deinit();
				pointer.gameObject.SetActive(false);
			}
		}
	}
}