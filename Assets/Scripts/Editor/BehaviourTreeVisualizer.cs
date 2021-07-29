using UnityEditor;
using UnityEngine;

using System.Collections.Generic;
using System.Reflection;

using STP.Config;
using STP.Utils.BehaviourTree;
using STP.Utils.BehaviourTree.Tasks;

using XNode;
using XNodeEditor;

namespace STP.Editor {
	public class BehaviourTreeVisualizer : EditorWindow {
		// Add menu named "My Window" to the Window menu
		[MenuItem("STP/Behaviour tree visualizer")]
		static void InitWindow() {
			// Get existing open window or if none, make a new one:
			var window = GetWindow<BehaviourTreeVisualizer>();
			window.Show();
		}

		BehaviourTree _openedTree;
		NodeGraph     _graph;
		
		void OnGUI() {
			var trees = GetBehaviourTrees();
			if ( (trees.Count == 0) || (trees[0] == _openedTree)) {
				return;
			}
			if ( _openedTree != null ) {
				_openedTree.OnBehaviourTreeUpdated -= OnBehaviourTreeUpdate;
			}
			_openedTree                        =  trees[0];
			_openedTree.OnBehaviourTreeUpdated += OnBehaviourTreeUpdate;
			_graph                             =  DrawBehaviourTree(_openedTree);
		}

		void OnBehaviourTreeUpdate(BehaviourTree tree) {
			RefreshStatuses();
		}

		void RefreshStatuses() {
			if ( !_graph ) {
				return;
			}
			foreach ( var node in _graph.nodes ) {
				if ( node is BehaviourTreeNode btNode ) {
					btNode.UpdateValues();
				}
			}
		}

		NodeGraph DrawBehaviourTree(BehaviourTree tree) {
			var graph = CreateGraphForBehaviourTree(tree);
			NodeEditorWindow.Open(graph);
			return graph;
		}

		NodeGraph CreateGraphForBehaviourTree(BehaviourTree tree) {
			var graph    = CreateInstance<BehaviourTreeGraph>();
			_layer = 0;
			var rootNode = CreateNode(graph, tree.Root, null);
			_layer = 1;
			CreateSubGraph(graph, tree.Root, rootNode);
			return graph;
		}

		int _layer        = 0;
		int _inLayerIndex = 0;

		float _itemHeight = 300;
		float _itemWidth  = 300;

		void CreateSubGraph(BehaviourTreeGraph graph, BaseTask task, BehaviourTreeNode parent) {
			var nodes = new List<BehaviourTreeNode>();
			
			_inLayerIndex = 0;
			foreach ( var subTask in task.SubTasks ) {
				nodes.Add(CreateNode(graph, subTask, parent));
				_inLayerIndex++;
			}

			_inLayerIndex = 0;
			
			_layer++;
			for ( var i = 0; i < nodes.Count; i++ ) {
				CreateSubGraph(graph, task.SubTasks[i], nodes[i]);
			}
			_layer--;
		}


		BehaviourTreeNode CreateNode(BehaviourTreeGraph graph, BaseTask task, BehaviourTreeNode parent) {
			var node = graph.AddNode<BehaviourTreeNode>();
			node.ThisNode = node;
			node.Parent   = parent;
			node.name     = task.GetType().Name;
			node.Task     = task;
			NodePort parentOutputPort = null;
			if ( parent != null ) {
				foreach ( var port in parent.Outputs ) {
					if ( port.fieldName == nameof(node.ThisNode)) {
						parentOutputPort = port;
					}
				}
			}
			if ( parentOutputPort != null ) {
				foreach ( var port in node.Inputs ) {
					if ( port.fieldName == nameof(node.Parent)) {
						parentOutputPort.Connect(port);
					}
				}
			}
			node.position = new Vector2(_layer * _itemWidth, _inLayerIndex * _itemHeight);
			node.UpdateValues();
			return node;
		}
		
		

		List<BehaviourTree> GetBehaviourTrees() {
			var res = new List<BehaviourTree>();
			foreach ( var obj in Selection.objects ) {
				if ( obj is GameObject go ) {
					var trees = FindBehaviourTreesInGameObject(go);
					res.AddRange(trees);
				}
			}
			return res;
		}

		List<BehaviourTree> FindBehaviourTreesInGameObject(GameObject obj) {
			var res = new List<BehaviourTree>();
			foreach ( var component in obj.GetComponents<Component>() ) {
				var behaviourTrees = FindBehaviourTreesInObject(component);
				res.AddRange(behaviourTrees);	
			}
			return res;
		}

		List<BehaviourTree> FindBehaviourTreesInObject(object obj) {
			var res    = new List<BehaviourTree>();
			var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach ( var field in fields ) {
				var bt = GetBehaviourTreeFromField(obj, field);
				if ( bt != null ) {
					res.Add(bt);
				}
			}
			return res;
		}

		BehaviourTree GetBehaviourTreeFromField(object obj, FieldInfo fieldInfo) {
			if ( fieldInfo.FieldType != typeof(BehaviourTree) ) {
				return null;
			}
			return fieldInfo.GetValue(obj) as BehaviourTree;
		}
	}
}