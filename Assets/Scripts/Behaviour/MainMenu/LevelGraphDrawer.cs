using UnityEngine;

using System.Collections.Generic;

using STP.Config;

namespace STP.Behaviour.MainMenu {
	public class LevelGraphDrawer {
		readonly Dictionary<LevelNode, MainLevelUiContainer> _createdButtons = new Dictionary<LevelNode, MainLevelUiContainer>();
		readonly Dictionary<int, GameObject> _layerRoots = new Dictionary<int, GameObject>();

		GameObject _levelButtonPrefab;
		GameObject _layerPrefab;

		StartLevelNode _startLevelNode;
		GameObject     _graphRoot;
		
		public void InitGraph(GameObject layerPrefab, GameObject levelButtonPrefab, GameObject graphRoot, StartLevelNode startLevelNode) {
			_levelButtonPrefab = levelButtonPrefab;
			_layerPrefab       = layerPrefab;
			_startLevelNode    = startLevelNode;
			_graphRoot         = graphRoot;
		}
		
		public void DrawGraph() {
			DrawSubGraph(_startLevelNode);
		}

		void DrawSubGraph(LevelNode root) {
			TryCreateMainNode(root);
			foreach ( var level in root.NextLevels ) {
				DrawSubGraph(level);
			}
			TryInitOptionalLevelNodes(root);
		}

		void TryInitOptionalLevelNodes(LevelNode root) {
			//Draw optional levels
			var mainNodeButtonContainer = _createdButtons[root];
			for ( var i = 0; i < mainNodeButtonContainer.OptionalLevels.Count; i++ ) {
				mainNodeButtonContainer.OptionalLevels[i].gameObject.SetActive(i < root.OptionalLevels.Count);
			}
		}
		
		void TryCreateMainNode(LevelNode node) {
			if ( _createdButtons.ContainsKey(node) ) {
				return;
			}
			var nodeLayer = GetMaxPathToNode(_startLevelNode, node);
			Debug.Log($"node: {node.name}, layer: {nodeLayer}");
			var layerGo = GetOrCreateLayer(nodeLayer);
			var button  = CreateMainRoadLevelButton(layerGo.transform);
			_createdButtons.Add(node, button);
		}

		MainLevelUiContainer CreateMainRoadLevelButton(Transform root) {
			var buttonGo = GameObject.Instantiate(_levelButtonPrefab, root);
			return buttonGo.transform.GetComponent<MainLevelUiContainer>();
		}

		GameObject GetOrCreateLayer(int layerIndex) {
			if ( _layerRoots.ContainsKey(layerIndex) ) {
				return _layerRoots[layerIndex];
			}
			var layerGo = GameObject.Instantiate(_layerPrefab, _graphRoot.transform);
			_layerRoots.Add(layerIndex, layerGo);
			return layerGo;
		}

		int GetMaxPathToNode(LevelNode curNode, LevelNode dst) {
			if ( curNode.NextLevels.Contains(dst) ) {
				return 1;
			}
			if ( curNode == dst ) {
				return 0;
			}
			var max = -1;
			foreach ( var level in curNode.NextLevels ) {
				var path = GetMaxPathToNode(level, dst);
				if ( path == -1 ) {
					continue;
				}
				max = Mathf.Max(path + 1, max);
			}
			return max;
		}
	}
}