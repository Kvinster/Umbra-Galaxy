﻿using UnityEngine;

using System.Collections.Generic;

namespace STP.Manager {
	public sealed class PauseManager {
		readonly HashSet<object> _pauseHolders = new HashSet<object>();

		public void Deinit() {
			Time.timeScale = 1f;
		}

		public void Pause(object holder) {
			if ( holder == null ) {
				Debug.LogError("Holder is null");
				return;
			}
			_pauseHolders.Add(holder);
			Time.timeScale = 0f;
		}

		public void Unpause(object holder) {
			if ( _pauseHolders.Remove(holder) && (_pauseHolders.Count == 0) ) {
				Time.timeScale = 1f;
			}
		}
	}
}