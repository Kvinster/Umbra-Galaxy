using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Starter;
using STP.Manager;

namespace STP.Behaviour.Core.UI {
	public class GeneratorPointersController : BaseCoreComponent {
		public List<Pointer> Pointers;

		Portal       _portal;
		LevelManager _levelManager;

		void OnDestroy() {
			GeneratorsWatcher.GeneratorsCountChanged -= OnGeneratorsCountChanged;
			if ( _levelManager != null ) {
				_levelManager.OnLevelWinStarted -= OnLevelWon;
			}
		}

		protected override void InitInternal(CoreStarter starter) {
			_portal                         =  starter.Portal;
			_levelManager                   =  starter.LevelManager;
			_levelManager.OnLevelWinStarted += OnLevelWon;

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

		void OnLevelWon() {
			var pointer = Pointers[0];
			pointer.Init(_portal.transform);
			pointer.gameObject.SetActive(true);
		}
	}
}