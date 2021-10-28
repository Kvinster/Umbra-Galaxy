using UnityEngine;

using System.Collections.Generic;

using STP.Behaviour.Core.Enemy.GeneratorEditor;
using STP.Utils;

namespace STP.Behaviour.Core.Generators {
	public sealed class EditorTimeRegularLevelGenerator : GameComponent {
		[Header("Parameters")]
		public Vector2Int GeneratorsSideSize = new Vector2Int(5, 5);
		[Header("Dependencies")]
		public Transform LevelObjectsRoot;
		public GameObject GeneratorBulletPrefab;
		public GameObject MainGeneratorBulletPrefab;
		[Space]
		public List<GameObject> BulletPrefabs = new List<GameObject>();

		void Start() {
			if ( Application.isPlaying ) {
				Debug.LogWarning("EditorTimeRegularLevelGenerator.Start: not supposed to exist in runtime");
			}
		}

		public void GenerateLevel() {
			ResetLevel();
			var levelObjectsRootGo = new GameObject("LevelObjects");
			LevelObjectsRoot = levelObjectsRootGo.transform;
			LevelObjectsRoot.position = Vector3.zero;
			var chunkCreator = new ChunkCreator(GeneratorBulletPrefab, MainGeneratorBulletPrefab);
			var obj          = chunkCreator.CreateGeneratorChunk(GeneratorsSideSize);
			obj.transform.SetParent(LevelObjectsRoot);
			obj.transform.localPosition = Vector3.zero;
		}

		public void ResetLevel() {
			if ( LevelObjectsRoot ) {
				DestroyImmediate(LevelObjectsRoot.gameObject);
				LevelObjectsRoot = null;
			}
		}

		public void ResetField() {
			LevelObjectsRoot = null;
		}
	}
}
