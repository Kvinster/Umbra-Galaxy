using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

using System;

using STP.Behaviour.Core.Generators;

namespace STP.Editor {
	[CustomEditor(typeof(EditorTimeRegularLevelGenerator))]
	public sealed class EditorTimeRegularLevelGeneratorEditor : UnityEditor.Editor {
		const string BulletPrefabsPropertyName = nameof(EditorTimeRegularLevelGenerator.BulletPrefabs);
		const string GeneratorBulletPrefabPropertyName =
			nameof(EditorTimeRegularLevelGenerator.GeneratorBulletPrefab);
		const string MainGeneratorBulletPrefabPropertyName =
			nameof(EditorTimeRegularLevelGenerator.MainGeneratorBulletPrefab);

		public override void OnInspectorGUI() {
			serializedObject.Update();
			var generator = target as EditorTimeRegularLevelGenerator;
			var bulletPrefabsProperty =
				serializedObject.FindProperty(nameof(EditorTimeRegularLevelGenerator.BulletPrefabs));
			var prefabs     = new GameObject[bulletPrefabsProperty.arraySize + 1];
			var prefabNames = new string[bulletPrefabsProperty.arraySize + 1];
			prefabs[0]     = null;
			prefabNames[0] = "_None";
			if ( bulletPrefabsProperty.arraySize > 0 ) {
				for ( var i = 0; i < bulletPrefabsProperty.arraySize; ++i ) {
					var obj = bulletPrefabsProperty.GetArrayElementAtIndex(i).objectReferenceValue;
					if ( obj is GameObject go ) {
						prefabs[i + 1]     = go;
						prefabNames[i + 1] = go.name;
					} else {
						Debug.LogErrorFormat("OnInspectorGUI: Unexpected value '{0}'", obj);
						return;
					}
				}
			}
			DrawPropertiesExcluding(serializedObject, BulletPrefabsPropertyName, GeneratorBulletPrefabPropertyName, MainGeneratorBulletPrefabPropertyName);
			DrawPrefabsList(GeneratorBulletPrefabPropertyName, prefabNames, prefabs);
			DrawPrefabsList(MainGeneratorBulletPrefabPropertyName, prefabNames, prefabs);
			EditorGUILayout.PropertyField(serializedObject.FindProperty(BulletPrefabsPropertyName));
			serializedObject.ApplyModifiedProperties();

			if ( GUILayout.Button("Generate level") ) {
				InvokeGeneratorMethod(nameof(EditorTimeRegularLevelGenerator.GenerateLevel));
			}
			if ( GUILayout.Button("Reset level") ) {
				InvokeGeneratorMethod(nameof(EditorTimeRegularLevelGenerator.ResetLevel));
			}
			if ( GUILayout.Button("Reset field") ) {
				InvokeGeneratorMethod(nameof(EditorTimeRegularLevelGenerator.ResetField));
			}
		}

		void DrawPrefabsList(string propertyName, string[] prefabNames, GameObject[] prefabs) {
			var property = serializedObject.FindProperty(propertyName);
			Assert.IsNotNull(property);
			var selectedPrefab = property.objectReferenceValue;
			var selectedIndex  = (selectedPrefab is GameObject go) ? Array.IndexOf(prefabNames, go.name) : 0;
			var newIndex = EditorGUILayout.Popup(property.displayName, selectedIndex, prefabNames);
			if ( newIndex != selectedIndex ) {
				var prefab = prefabs[newIndex];
				property.objectReferenceValue = prefab;
			}
		}

		void InvokeGeneratorMethod(string methodName) {
			var type   = typeof(EditorTimeRegularLevelGenerator);
			var method = type.GetMethod(methodName);
			if ( method == null ) {
				Debug.LogErrorFormat("Can't find method '{0}' in type '{1}'", methodName, type.Name);
				return;
			}
			method.Invoke(serializedObject.targetObject, new object[] { });
		}
	}
}
