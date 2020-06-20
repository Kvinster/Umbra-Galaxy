using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STP.Utils {
    public class ProblemChecker {
        static readonly Stack<int> _indexStack = new Stack<int>();
		
        static MonoBehaviour _context;
		
        public static void LogErrorIfNullOrEmpty(MonoBehaviour context, params object[] objectsToCheck) {
            _context = context;
            for (var i = 0; i < objectsToCheck.Length; i++) {
                _indexStack.Push(i);
                CheckUnknownObject(objectsToCheck[i]);
                _indexStack.Pop();
            }
        }
		
        static void CheckUnknownObject(object obj) {
            switch ( obj ) {
                case Object unityObject: {
                    CheckUnityObject(unityObject);
                    break;
                }
                case ICollection collection: {
                    CheckCollection(collection);
                    break;
                }
                case null: {
                    PrintError();
                    break;
                }
            }
        }

        static void CheckUnityObject(Object unityObject) {
            if ( !unityObject ) {
                PrintError();
            }
        }
		
        static void CheckCollection(ICollection collection) {
            if ( collection.Count == 0) {
                PrintError();
                return;
            }

            var index = 0;
            foreach ( var element in collection ) {
                _indexStack.Push(index);
                CheckUnknownObject(element);
                _indexStack.Pop();
                index++;
            }
        }

        static void PrintError() {
            var sb = new StringBuilder();
            sb.Append("Missing ");
            foreach ( var index in _indexStack.Reverse() ) {
                sb.Append(index);
                sb.Append("->");
            }
            sb.Remove(sb.Length - 2, 2);
            Debug.LogError(sb, _context);
        }
    }
}
