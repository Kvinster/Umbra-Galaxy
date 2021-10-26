using UnityEngine;

using STP.Behaviour.Core.Enemy.GeneratorEditor;
using STP.Utils;

using JetBrains.Annotations;
using NaughtyAttributes;

namespace STP.Behaviour.Core.Generators {
	public sealed class EditorTimeRegularLevelGenerator : GameComponent {
		[Header("Parameters")]
		public Vector2Int GeneratorsSideSize = new Vector2Int(5, 5);
		[Header("Dependencies")]
		public Transform LevelObjectsRoot;

		void Start() {
			if ( Application.isPlaying ) {
				Debug.LogWarning("EditorTimeRegularLevelGenerator.Start: not supposed to exist in runtime");
			}
		}

		[Button("Generate level")]
		[UsedImplicitly]
		public void GenerateLevel() {
			ResetLevel();
			var levelObjectsRootGo = new GameObject("LevelObjects");
			LevelObjectsRoot = levelObjectsRootGo.transform;
			LevelObjectsRoot.position = Vector3.zero;
			var chunkCreator = new ChunkCreator();
			var obj          = chunkCreator.CreateGeneratorChunk(GeneratorsSideSize);
			obj.transform.SetParent(LevelObjectsRoot);
			obj.transform.localPosition = Vector3.zero;
		}

		[Button("Reset level")]
		[UsedImplicitly]
		public void ResetLevel() {
			if ( LevelObjectsRoot ) {
				DestroyImmediate(LevelObjectsRoot.gameObject);
				LevelObjectsRoot = null;
			}
		}

		[Button("Reset field")]
		[UsedImplicitly]
		public void ResetField() {
			LevelObjectsRoot = null;
		}
	}
}
