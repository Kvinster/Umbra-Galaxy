using System.Collections.Generic;
using System.Reflection;
using STP.Config;
using STP.Utils.BehaviourTree.Tasks;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

namespace STP.Utils.BehaviourTree.Editor.Visualiser {
	public class BehaviourTreeDrawer : NodeGraphEditor {
		BehaviourTree _openedTree;

		NodeGraph _graph;
		
		public void SetBehaviourTree(BehaviourTree tree) {
			if ( tree.Root == null ) {
				return;
			}
			if ( _openedTree != null ) {
				_openedTree.OnBehaviourTreeUpdated -= OnBehaviourTreeUpdate;
			}
			_openedTree                        =  tree;
			_openedTree.OnBehaviourTreeUpdated += OnBehaviourTreeUpdate;
			_graph                             =  CreateGraphForBehaviourTree(_openedTree);
			NodeEditorWindow.Open(_graph);
		}

		void OnBehaviourTreeUpdate(BehaviourTree tree) {
			RefreshStatuses();
			if ( EditorWindow.HasOpenInstances<NodeEditorWindow>() ) {
				NodeEditorWindow.RepaintAll();
			}
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

		NodeGraph CreateGraphForBehaviourTree(BehaviourTree tree) {
			var graph    = ScriptableObject.CreateInstance<BehaviourTreeGraph>();
			CreateSubGraph(graph, tree.Root);
			return graph;
		}

		int _layer;
		int _inLayerIndex;

		float _itemHeight = 200;
		float _itemWidth  = 300;

		void CreateSubGraph(BehaviourTreeGraph graph, BaseTask root) {
			var visualNodes = new List<BehaviourTreeNode>();

			var nextLayerNodes    = new List<(BaseTask task, BaseTask parent)> {(root, null)};

			_layer = 0;
			while ( nextLayerNodes.Count > 0 ) {
				_inLayerIndex     = 0;
				var currentLayerNodes = nextLayerNodes;
				nextLayerNodes    = new List<(BaseTask task, BaseTask parent)>();
				foreach ( var taskPair in currentLayerNodes ) {
					var parentNode = visualNodes.Find(x => (x.Task == taskPair.parent));
					visualNodes.Add(CreateNode(graph, taskPair.task, parentNode));
					nextLayerNodes.AddRange(GetTaskChild(taskPair.task));
				}
				_layer++;
			}
		}

		List<(BaseTask task, BaseTask parent)> GetTaskChild(BaseTask task) {
			var res = new List<(BaseTask task, BaseTask parent)>();
			foreach ( var child in task.SubTasks ) {
				res.Add((child, task));
			}
			return res;
		}

		BehaviourTreeNode CreateNode(BehaviourTreeGraph graph, BaseTask task, BehaviourTreeNode parent) {
			var node = graph.AddNode<BehaviourTreeNode>();
			node.ThisNode = node;
			node.Parent   = parent;
			node.name     = task.TaskName;
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
			_inLayerIndex++;
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