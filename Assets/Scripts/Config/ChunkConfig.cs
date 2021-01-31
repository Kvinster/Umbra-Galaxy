#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

using System;
using System.Collections.Generic;
using System.IO;

namespace STP.Config {
	[CreateAssetMenu(fileName = "ChunkConfig", menuName = "ScriptableObjects/ChunkConfig", order = 1)]
	public class ChunkConfig : ScriptableObject {
		public const string ChunkConfigPath = "ChunkConfig";
		public const string SafeRectName    = "SafeChunk";

		const  string ChunksBasePath  = "Assets/Prefabs/LevelChunks/";

		[Serializable]
		public class ChunkInfo {
			public string     Name;
			public GameObject Prefab;
		}

		public List<ChunkInfo> ChunkInfos;

		public GameObject GetChunk(string chunkName) {
			return GetChunkInfo(chunkName)?.Prefab;
		}

		ChunkInfo GetChunkInfo(string chunkName) {
			return ChunkInfos.Find(x => x.Name == chunkName);
		}

		#if UNITY_EDITOR
		[ContextMenu("Generate config from files")]
		public void RegenerateConfig() {
			ChunkInfos.Clear();
			ProcessDirectory(ChunksBasePath);
		}

		void ProcessDirectory(string path) {
			var files = Directory.GetFiles(path);
			var dirs  = Directory.GetDirectories(path);

			foreach ( var file in files ) {
				ProcessFile(file);
			}

			foreach ( var dir in dirs ) {
				ProcessDirectory(dir);
			}
		}

		void ProcessFile(string path) {
			if ( path.EndsWith(".meta") ) {
				return;
			}

			var name = (path.Contains(SafeRectName))
				? SafeRectName
				: path.Replace(ChunksBasePath, string.Empty).Replace(".prefab", string.Empty);
			var chunkInfo = new ChunkInfo {
				Name   = name,
				Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path)
			};
			ChunkInfos.Add(chunkInfo);
		}
		#endif
 	}
}