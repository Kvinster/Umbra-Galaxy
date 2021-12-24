using UnityEngine;

using System.Collections.Generic;
using NaughtyAttributes;
using STP.Behaviour.Core.Enemy;
using STP.Behaviour.Core.Enemy.GeneratorEditor;
using STP.Behaviour.Starter;
using STP.Utils;

namespace STP.Behaviour.Core.Generators {
	public sealed class EditorTimeRegularLevelGenerator : GameComponent {
		[Header("Parameters")]
		public Vector2Int GeneratorsSideSize = new Vector2Int(5, 5);
		[Header("Dependencies")]
		public GameObject GeneratorBulletPrefab;
		public float GeneratorReloadTime = 1f;

		public GameObject MainGeneratorBulletPrefab;
		public float MainGeneratorReloadTime = 0.5f;

		[Space]
		public List<GameObject> BulletPrefabs = new List<GameObject>();

		public float AreaAdditionalSizes = 500f;

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

			foreach (var generator in obj.GetComponentsInChildren<Generator>()) {
				generator.ShootingParams.ReloadTime = (generator.IsMainGenerator) ? MainGeneratorReloadTime : GeneratorReloadTime;

			}

			obj.transform.SetParent(_levelObjectsRoot);
			obj.transform.localPosition = Vector3.zero;
		}

		public void RecalculateArea() {
			var levelObjectsRootGo  = GameObject.Find("LevelObjects");
			if (!levelObjectsRootGo) {
				Debug.LogError("Can't find level objects root. Aborted");
				return;
			}
			var starter = GameObject.Find("Starter");
			if (!starter) {
				Debug.LogError("Can't find core starter. Aborted");
				return;
			}
			var starterComp = starter.GetComponent<CoreStarter>();

			var maxPosition = new Vector2(float.MinValue, float.MinValue);
			var minPosition = new Vector2(float.MaxValue, float.MaxValue);

			foreach (var obj in levelObjectsRootGo.GetComponentsInChildren<Transform>()) {
				var objPosition = obj.position;
				minPosition = Vector2.Min(minPosition, objPosition);
				maxPosition = Vector2.Max(maxPosition, objPosition);
			}

			var offset = new Vector2(AreaAdditionalSizes, AreaAdditionalSizes);

			maxPosition += offset;
			minPosition -= offset;

			starterComp.AreaRect = new Rect(minPosition, maxPosition - minPosition);

		}
	}
}