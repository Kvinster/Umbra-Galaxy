using UnityEngine;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	public class Map<T> {
		T[,] _map;

		public int Size => _map.GetLength(0);

		public Map(int size) {
			_map = new T[size, size];
		}

		public T GetCell(int x, int y) {
			return IsOnMap(x, y) ? _map[x, y] : default(T);
		}

		public void SetCell(int x, int y, T state) {
			if ( !IsOnMap(x, y) ) {
				Debug.LogError($"Trying to set incorrect cell: ({x} {y})");
				return;
			}
			_map[x, y] = state;
		}

		bool IsOnMap(int x, int y) {
			return (x >= 0) && (y >= 0) && (x < _map.GetLength(0)) && (y < _map.GetLength(1));
		}

	}
}