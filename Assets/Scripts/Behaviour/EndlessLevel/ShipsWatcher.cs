using UnityEngine;

using System;
using System.Collections.Generic;
using STP.Behaviour.EndlessLevel.Enemies;

namespace STP.Behaviour.EndlessLevel {
	public class ShipsWatcher {
		public event Action OnAllShipsDestroyed;

		readonly List<BaseEnemy> _ships = new List<BaseEnemy>();
		bool _eventFired;
		
		public void Tick() {
			if ( _eventFired || (_ships.Count != 0) ) {
				return;
			}
			_eventFired = true;
			OnAllShipsDestroyed?.Invoke();
		}
		
		public void RegisterShip(BaseEnemy ship) {
			if ( _ships.Contains(ship) ) {
				Debug.LogError("Can't add ship. Ship was already added");
				return;
			}
			_eventFired = false;
			_ships.Add(ship);
			ship.OnDestroyed += HandleShipDeath;
		}

		public void WatchWave() {
			_eventFired = false;
		}
		
		void HandleShipDeath(BaseEnemy ship) {
			ship.OnDestroyed -= HandleShipDeath;
			_ships.Remove(ship);
		}
	}
}