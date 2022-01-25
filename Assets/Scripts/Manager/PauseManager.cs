using UnityEngine;
using UnityEngine.Assertions;

using System;
using System.Collections.Generic;

namespace STP.Manager {
	public sealed class PauseManager {
		public static PauseManager Instance { get; private set; }

		readonly List<object> _pauseHolders = new List<object>();

		public bool IsPaused => (_pauseHolders.Count > 0);

		public event Action<bool> OnIsPausedChanged;

		public PauseManager() {
			Assert.IsNull(Instance);
			Instance = this;
		}

		public void Deinit() {
			Assert.IsTrue(Instance == this);
			Time.timeScale = 1f;
			Instance       = null;
		}

		public void Pause(object holder) {
			if ( holder == null ) {
				Debug.LogError("Holder is null");
				return;
			}
			_pauseHolders.Add(holder);
			Time.timeScale = 0f;
			if ( _pauseHolders.Count == 1 ) {
				OnIsPausedChanged?.Invoke(true);
			}
		}

		public void Unpause(object holder) {
			if ( _pauseHolders.Remove(holder) && (_pauseHolders.Count == 0) ) {
				Time.timeScale = 1f;
				OnIsPausedChanged?.Invoke(false);
			}
		}
	}
}
