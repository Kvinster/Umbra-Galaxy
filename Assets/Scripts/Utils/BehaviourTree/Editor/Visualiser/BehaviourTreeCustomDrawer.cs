using UnityEditor;
using UnityEngine;

using System.Reflection;

namespace STP.Utils.BehaviourTree.Editor.Visualiser {
	
	[CustomPropertyDrawer(typeof(BehaviourTree))]
	public class BehaviourTreeCustomDrawer : PropertyDrawer {
		BehaviourTreeDrawer _drawer = new BehaviourTreeDrawer();
		
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			var tree = GetManagedReference<BehaviourTree>(property, property.name);
			if ( GUI.Button(position, "Visualize tree") ) {
				_drawer.SetBehaviourTree(tree);
			}
		}

		T GetManagedReference<T>(SerializedProperty property, string fieldName) where T : class {
			var parent     = property.serializedObject.targetObject;
			var parentType = parent.GetType();
			var fieldType =
				parentType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return fieldType?.GetValue(parent) as T;
		}
	}
}