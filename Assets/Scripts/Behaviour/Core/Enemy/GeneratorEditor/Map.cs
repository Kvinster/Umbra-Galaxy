using UnityEngine;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	public class Map<T> {
		readonly Vector2Int _size;
		readonly T[,]       _map;

		public Vector2Int Size => _size;

		public Map(int size) : this(new Vector2Int(size, size)) { }

		public Map(Vector2Int size) {
			_size = size;
			_map  = new T[_size.x, _size.y];
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
			return (x >= 0) && (y >= 0) && (x < Size.x) && (y < Size.y);
		}

	}
}