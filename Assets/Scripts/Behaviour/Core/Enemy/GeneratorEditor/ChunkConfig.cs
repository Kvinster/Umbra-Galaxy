using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Behaviour.Core.Enemy.GeneratorEditor {
	[CreateAssetMenu(menuName = "ScriptableObjects/" + nameof(ChunkConfig), fileName = nameof(ChunkConfig))]
	public sealed class ChunkConfig : ScriptableObject {
		[Serializable]
		public sealed class BulletPair {
			public GameObject MainGenBullet;
			public GameObject SubGenBullet;
		}

		[Header("For generator")]
		public GameObject       MainGeneratorPrefab;
		public GameObject       GeneratorPrefab;
		public List<BulletPair> BulletPrefabs;
		public GameObject       ConnectorPrefab;
		public GameObject       LinePrefab;

		[Header("Specific chunks")]
		public GameObject       SafeAreaPrefab;
		public List<GameObject> IdleChunks;
	}
}
