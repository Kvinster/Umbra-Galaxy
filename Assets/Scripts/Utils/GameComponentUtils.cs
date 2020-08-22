using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Experimental.SceneManagement;
#endif
using System;
using System.Collections;
using System.Reflection;

using STP.Utils.GameComponentAttributes;

using Object = UnityEngine.Object;
 
namespace STP.Utils {
	public static class GameComponentUtils {
		public static void CheckAttributes(object rawObj, Object context = null) {
#if UNITY_EDITOR
			if ( !context ) {
				context = rawObj as Object;
			}
			var isPrefab = false;
			if ( context is MonoBehaviour mb ) {
				isPrefab = string.IsNullOrEmpty(mb.gameObject.scene.name) ||
				           (PrefabStageUtility.GetCurrentPrefabStage() != null);
			}
			foreach ( var fieldInfo in rawObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance) ) {
				foreach ( var attributeRaw in fieldInfo.GetCustomAttributes(true) ) {
					if ( isPrefab && (attributeRaw is BaseGameComponentAttribute bgca) && !bgca.CheckPrefab ) {
						continue;
					}
					switch ( attributeRaw ) {
						case NotNullAttribute _: {
							var valueRaw = fieldInfo.GetValue(rawObj);
							if ( (valueRaw == null) || ((valueRaw is Object unityObj) && !unityObj) ) {
								Debug.LogErrorFormat(context, "'{0}' is null", fieldInfo.Name);
							}
							break;
						}
						case NotNullOrEmptyAttribute attr: {
							var valueRaw = fieldInfo.GetValue(rawObj);
							switch ( valueRaw ) {
								case ICollection collection: {
									if ( collection.Count == 0 ) {
										Debug.LogErrorFormat(context, "Collection '{0}' is empty",
											fieldInfo.Name);
									}
									if ( !attr.AllowNullNodes ) {
										foreach ( var node in collection ) {
											if ( node is Object unityObj ) {
												if ( !unityObj ) {
													Debug.LogErrorFormat(context,
														"Collection '{0}' has null elements", fieldInfo.Name);
													break;
												}
											} else if ( node == null ) {
												Debug.LogErrorFormat(context,
													"Collection '{0}' has null elements", fieldInfo.Name);
												break;
											}
										}
									}
									break;
								}
								case Object valueObj: {
									if ( !valueObj ) {
										Debug.LogErrorFormat(context, "'{0}' is null",
											fieldInfo.Name);
									}
									break;
								}
								case string str: {
									if ( string.IsNullOrEmpty(str) ) {
										Debug.LogErrorFormat(context, "'{0}' is null or empty",
											fieldInfo.Name);
									}
									break;
								}
								default: {
									if ( valueRaw == null ) {
										Debug.LogErrorFormat(context, "'{0}' is null",
											fieldInfo.Name);
									}
									break;
								}
							}
							break;
						}
						case CountAttribute attr: {
							var valueRaw = fieldInfo.GetValue(rawObj);
							if ( valueRaw is ICollection collection ) {
								if ( (attr.MinCount > 0) && (collection.Count < attr.MinCount) ) {
									Debug.LogErrorFormat(context,
										"Collection '{0}' has less than '{1}' elements", fieldInfo.Name,
										attr.MinCount);
								}
								if ( (attr.MaxCount > 0) && (collection.Count > attr.MaxCount) ) {
									Debug.LogErrorFormat(context,
										"Collection '{0}' has more than '{1}' elements", fieldInfo.Name,
										attr.MaxCount);
								}
								if ( (attr.ExactCount > 0) && (collection.Count != attr.ExactCount) ) {
									Debug.LogErrorFormat(context,
										"Collection '{0}' must have '{1}' elements", fieldInfo.Name,
										attr.ExactCount);
								}
							} else {
								Debug.LogErrorFormat(context,
									"Field '{0}' has invalid type for attribute Count", fieldInfo.Name);
							}
							break;
						}
						case EnumNotEqualsAttribute attr: {
							if ( fieldInfo.FieldType.IsEnum ) {
								if ( (int)fieldInfo.GetValue(rawObj) == attr.EnumIntValue ) {
									var valueRaw = fieldInfo.GetValue(rawObj);
									Debug.LogErrorFormat(context,
										"Field '{0}' mustn't be equal to '{1}'", fieldInfo.Name,
										Enum.Parse(fieldInfo.FieldType, valueRaw.ToString()));
								}
							} else {
								Debug.LogErrorFormat(context,
									"Field '{0}' has invalid type for attribute EnumNotEquals", fieldInfo.Name);
							}
							break;
						}
					}
				}
			}
#endif
		}
	}
}