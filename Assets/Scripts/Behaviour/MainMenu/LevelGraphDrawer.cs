using UnityEngine;

using System.Collections.Generic;

using STP.Config;
using STP.Core;

using Cysharp.Threading.Tasks;
using Shapes;

namespace STP.Behaviour.MainMenu {
	public class LevelGraphDrawer {
		readonly Dictionary<int, List<LevelNode>>  _nodeLayers   = new Dictionary<int, List<LevelNode>>();
		readonly Dictionary<LevelNode, GameObject> _levelButtons = new Dictionary<LevelNode, GameObject>();

		readonly Dictionary<int, GameObject> _layerRoots = new Dictionary<int, GameObject>();

		LevelController _levelController;
		
		GameObject _levelButtonPrefab;
		GameObject _layerPrefab;

		StartLevelNode _startLevelNode;
		RectTransform  _graphRoot;

		public void InitGraph(LevelController levelController, GameObject layerPrefab, GameObject levelButtonPrefab,
			StartLevelNode startLevelNode, RectTransform graphRoot) {
			_levelButtonPrefab = levelButtonPrefab;
			_layerPrefab       = layerPrefab;
			_startLevelNode    = startLevelNode;
			_graphRoot         = graphRoot;
			_levelController   = levelController;
		}
		
		public void DrawGraph() {
			DistributeLevels(_startLevelNode);
			DrawLayers();
			UniTask.Void(DrawConnections);
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

		async UniTaskVoid DrawConnections() {
			await UniTask.Yield();
			foreach ( var nodeLayer in _nodeLayers ) {
				foreach ( var node in nodeLayer.Value ) {
					foreach ( var nextNode in node.NextLevels ) {
						DrawConnection(node, nextNode);
					}
					foreach ( var nextNode in node.OptionalLevels ) {
						DrawConnection(node, nextNode);
					}
				}
			}
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
			var line  = CreateLine(srcGo.transform);
			line.Thickness  = 10;
			line.End        = dstGo.transform.position - srcGo.transform.position;
			line.ColorMode  = Line.LineColorMode.Double;
			line.ColorStart = Color.red;
			line.ColorEnd   = Color.magenta;
		}

		Line CreateLine(Transform parent) {
			var go = new GameObject();
			go.transform.SetParent(parent);
			go.transform.localPosition = Vector3.zero;
			return go.AddComponent<Line>();
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
			var layerGo = GameObject.Instantiate(_layerPrefab, _graphRoot);
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