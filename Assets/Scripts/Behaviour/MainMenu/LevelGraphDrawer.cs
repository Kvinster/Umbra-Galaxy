using UnityEngine;

using System.Collections.Generic;

using STP.Config;
using STP.Core;

namespace STP.Behaviour.MainMenu {
	public class LevelGraphDrawer {
		readonly Dictionary<int, List<LevelNode>>  _nodeLayers   = new Dictionary<int, List<LevelNode>>();
		readonly Dictionary<LevelNode, GameObject> _levelButtons = new Dictionary<LevelNode, GameObject>();
		
		readonly Dictionary<int, GameObject> _layerRoots = new Dictionary<int, GameObject>();

		LevelController _levelController;
		
		GameObject _levelButtonPrefab;
		GameObject _layerPrefab;

		StartLevelNode _startLevelNode;
		GameObject     _graphRoot;
		
		public void InitGraph(LevelController levelController, GameObject layerPrefab, GameObject levelButtonPrefab, 
			GameObject graphRoot, StartLevelNode startLevelNode) {
			_levelButtonPrefab = levelButtonPrefab;
			_layerPrefab       = layerPrefab;
			_startLevelNode    = startLevelNode;
			_graphRoot         = graphRoot;
			_levelController   = levelController;
		}
		
		public void DrawGraph() {
			DistributeLevels(_startLevelNode);
			DrawLayers();
		}

		void DistributeLevels(LevelNode node) {
			TryAddNodeToDistribution(node);
			foreach ( var optionalLevel in node.OptionalLevels ) {
				TryAddNodeToDistribution(optionalLevel);
			}
			foreach ( var nextNode in node.NextLevels ) {
				DistributeLevels(nextNode);
			}
		}

		void TryAddNodeToDistribution(LevelNode levelNode) {
			var pathStepsCount = GetMaxPathToNode(_startLevelNode, levelNode);
			var layer = GetOrCreateNodeLayer(pathStepsCount);
			if ( layer.Contains(levelNode) ) {
				return;
			}
			layer.Add(levelNode);
		}

		List<LevelNode> GetOrCreateNodeLayer(int index) {
			if ( _nodeLayers.TryGetValue(index, out var res) ) {
				return res;
			}
			var newRes = new List<LevelNode>();
			_nodeLayers.Add(index, newRes);
			return newRes;
		}

		void DrawLayers() {
			for ( var layerIndex = 0; layerIndex < _nodeLayers.Count; layerIndex++ ) {
				DrawLayer(layerIndex);
			}
		}

		void DrawLayer(int layerIndex) {
			var layerGo    = GetOrCreateLayer(layerIndex);
			var layerNodes = _nodeLayers[layerIndex];
			foreach ( var layerNode in layerNodes ) {
				CreateLevelButton(layerGo.transform, layerNode);
			}
		}

		void DrawConnection(LevelNode srcNode, LevelNode dstNode) {
			var srcGo = _levelButtons[srcNode];
			var dstGo = _levelButtons[dstNode];
			
		}

		void CreateLevelButton(Transform root, LevelNode node) {
			var buttonGo = GameObject.Instantiate(_levelButtonPrefab, root);
			var buttonComp = buttonGo.transform.GetComponent<LevelButton>();
			buttonComp.Init(_levelController, node);
			_levelButtons.Add(node, buttonGo);
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
			if ( curNode.NextLevels.Contains(dst) || curNode.OptionalLevels.Contains(dst)) {
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