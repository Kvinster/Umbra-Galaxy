using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Enemy.GeneratorEditor;
using STP.Utils;

namespace STP.Behaviour.Core.Generators {
	public sealed class EditorTimeRegularLevelGenerator : GameComponent {
		[Header("Parameters")]
		public Vector2Int GeneratorsSideSize = new Vector2Int(5, 5);
		[Header("Dependencies")]
		public GameObject GeneratorBulletPrefab;
		public GameObject MainGeneratorBulletPrefab;
		[Space]
		public List<GameObject> BulletPrefabs = new List<GameObject>();

		Transform _levelObjectsRoot;

		void Start() {
			if ( Application.isPlaying ) {
				Debug.LogWarning("EditorTimeRegularLevelGenerator.Start: not supposed to exist in runtime");
			}
		}

		public void GenerateChunk() {
			var levelObjectsRootGo  = GameObject.Find("LevelObjects");
			if ( !levelObjectsRootGo ) {
				levelObjectsRootGo = new GameObject("LevelObjects");
			}
			_levelObjectsRoot = levelObjectsRootGo.transform;
			_levelObjectsRoot.position = Vector3.zero;
			var chunkCreator = new ChunkCreator(GeneratorBulletPrefab, MainGeneratorBulletPrefab);
			var obj          = chunkCreator.CreateGeneratorChunk(GeneratorsSideSize);
			obj.transform.SetParent(_levelObjectsRoot);
			obj.transform.localPosition = Vector3.zero;
		}

		public void ResetLevel() {
			if ( _levelObjectsRoot ) {
				DestroyImmediate(_levelObjectsRoot.gameObject);
				_levelObjectsRoot = null;
			}
		}

		public void ResetField() {
			_levelObjectsRoot = null;
		}
	}
}
