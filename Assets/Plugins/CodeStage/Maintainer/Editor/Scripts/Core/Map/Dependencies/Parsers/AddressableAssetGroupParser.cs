#region copyright
// ---------------------------------------------------------------
//  Copyright (C) Dmitriy Yukhanov - focus [https://codestage.net]
// ---------------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Core.Dependencies
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;

#if !UNITY_2019_2_OR_NEWER
	[InitializeOnLoad]
#endif
	// ReSharper disable once UnusedType.Global since it's used from TypeCache
	internal class AddressableAssetGroupParser : DependenciesParser
	{
		public override Type Type
		{
			get
			{
				return null;
			}
		}

#if !UNITY_2019_2_OR_NEWER
		static AddressableAssetGroupParser()
		{
			AssetDependenciesSearcher.AddInternalDependencyParser(new AddressableAssetGroupParser());
		}
#endif
		
		public override IList<string> GetDependenciesGUIDs(AssetInfo asset)
		{
			// checking by name since addressables are in optional external package
			if (asset.Type == null || asset.Type.Name != "AddressableAssetGroup")
				return null;

			var assetGroup = AssetDatabase.LoadMainAssetAtPath(asset.Path);
			return assetGroup == null ? null : ExtractReferencedAssets(assetGroup);
		}
		
		private static List<string> ExtractReferencedAssets(Object assetGroup)
		{
			var so = new SerializedObject(assetGroup);

			var serializedEntries = so.FindProperty("m_SerializeEntries");
			if (serializedEntries == null)
			{
				// legacy package version used this name
				serializedEntries = so.FindProperty("m_serializeEntries");

				if (serializedEntries == null)
				{
					Debug.LogError(Maintainer.ErrorForSupport("Can't reach serialize entries in AddressableAssetGroup!"));
					return null;
				}
			}

			if (!serializedEntries.isArray)
			{
				Debug.LogError(Maintainer.ErrorForSupport("Can't find serialize entries array in AddressableAssetGroup!"));
				return null;
			}

			var result = new List<string>();

			var count = serializedEntries.arraySize;
			for (var i = 0; i < count; i++)
			{
				var item = serializedEntries.GetArrayElementAtIndex(i);
				if (item == null)
				{
					Debug.LogWarning(Maintainer.ConstructLog("Serialize entry from AddressableAssetGroup is null!"));
					continue;
				}

				var referencedGUID = item.FindPropertyRelative("m_GUID");
				if (referencedGUID == null || referencedGUID.propertyType != SerializedPropertyType.String)
				{
					Debug.LogError(Maintainer.ErrorForSupport("Can't reach Serialize entry GUID of AddressableAssetGroup!"));
					return null;
				}

				var path = AssetDatabase.GUIDToAssetPath(referencedGUID.stringValue);
				if (!path.StartsWith("Assets"))
				{
					continue;
				}

				var guid = AssetDatabase.AssetPathToGUID(path);
				if (!string.IsNullOrEmpty(guid))
				{
					result.Add(guid);
				}
			}

			return result;
		}
	}
}