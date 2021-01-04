using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Core.Enemy;
using STP.Controller;
using STP.Utils;

using JetBrains.Annotations;
using NaughtyAttributes;

using Random = UnityEngine.Random;

namespace STP.Behaviour.Core {
	public class LevelGenerator : GameComponent {
		[Serializable]
		public class Chunk {
			public int             Weight;
			public GameObject      Prefab;
		}

		public int CellSideSize  = 300;

		public List<Chunk> LevelChunks;

		[Header("for editor button")] 
		public Transform Root;
		
		Dictionary<Chunk, int> _generatorsInChunks;

		int _needGenerators;

		[Button("Try generate level")] [UsedImplicitly]
		void GenerateLevelInEditor() {
			var randomSeed = Random.Range(int.MinValue, int.MaxValue);
			Random.InitState(randomSeed);
			for ( var i = Root.childCount-1; i >= 0; i-- ) {
				DestroyImmediate(Root.GetChild(i).gameObject);
			}
			GenerateLevel(LevelController.Instance, Root);
			Debug.Log($"Level generated. Seed {randomSeed}");
		}
		
		public void GenerateLevel(LevelController levelController, Transform root = null) {
			if ( _generatorsInChunks == null ) {
				ProcessChunks();
			}
			var levelConfig = levelController.GetCurLevelConfig();
			var spaceSize   = levelConfig.LevelSpaceSize;
			_needGenerators = levelConfig.GeneratorsCount;
			var minPoint    = new Vector2(-spaceSize / 2.0f, -spaceSize / 2.0f);
			var maxPoint    = new Vector2( spaceSize / 2.0f,  spaceSize / 2.0f);
			// Random.InitState(levelConfig.LevelSeed);
			var cellsCountX = Mathf.FloorToInt((maxPoint.x - minPoint.x) / CellSideSize);
			var cellsCountY = Mathf.FloorToInt((maxPoint.y - minPoint.y) / CellSideSize);
			var blocks      = new List<Vector2>();
			for ( var y = 0; y < cellsCountY; y++ ) {
				for ( var x = 0; x < cellsCountX; x++ ) {
					blocks.Add(new Vector2(x * CellSideSize, y * CellSideSize) + minPoint);
				}
			}
			var leftBlocks = blocks.Count;
			while ( leftBlocks > 0 ) {
				var randomBlockIndex = Random.Range(0, leftBlocks);
				var pos              = blocks[randomBlockIndex];
				var chunk            = GetRandomChunk();
				if ( chunk == null ) {
					Debug.LogError("Can't instanciate chunk. Chunk is null");
					continue;
				}
				Instantiate(chunk.Prefab, pos, Quaternion.identity, root);
				_needGenerators -= _generatorsInChunks[chunk];
				blocks.RemoveAt(randomBlockIndex);
				leftBlocks--;
			}
		}
		
		void ProcessChunks() {
			_generatorsInChunks = new Dictionary<Chunk, int>();
			foreach ( var chunk in LevelChunks ) {
				var generatorsCount = chunk.Prefab.GetComponentsInChildren<Generator>();
				_generatorsInChunks.Add(chunk, generatorsCount.Length);
			}
		}

		Chunk GetRandomChunk() {
			var totalWeight     = 0;
			var availableChunks = LevelChunks.FindAll(IsChunkAvailable);
			foreach ( var chunk in availableChunks ) {
				totalWeight += chunk.Weight;
			}
			var randomValue = Random.Range(0, totalWeight);
			foreach ( var chunk in availableChunks ) {
				if ( chunk.Weight > randomValue ) {
					return chunk;
				}

				randomValue -= chunk.Weight;
			}
			return null;
		}

		bool IsChunkAvailable(Chunk chunk) {
			if ( !_generatorsInChunks.ContainsKey(chunk) ) {
				Debug.LogError($"Can't find generators count for chunk {chunk.Prefab}");
				return false;
			}
			return _generatorsInChunks[chunk] <= _needGenerators;
		}
	}
}