using System;
using System.Collections.Generic;

namespace STP.Utils {
    public static class DijkstraPathFinder {
        public static (IEnumerable<string> path, int length) GetPath(string startNode, string finishNode,
            IEnumerable<string> nodes, Func<string, string, int> weightGetter,
            Func<string, List<string>> neighboursGetter) {
            var marks     = new Dictionary<string, int>();
            var paths     = new Dictionary<string, List<string>>();
            var listNodes = new List<string>(nodes);
            paths[startNode] = new List<string>();
            foreach ( var node in listNodes ) {
                marks.Add(node, (node == startNode) ? 0 : int.MaxValue);
            }
            var marked = new HashSet<string>();
            while ( listNodes.Count != marked.Count ) {
                string minNode = null;
                var minMark = int.MaxValue;
                foreach ( var node in listNodes ) {
                    if ( !marked.Contains(node) && (marks[node] < minMark) ) {
                        minMark = marks[node];
                        minNode = node;
                    }
                }
                marked.Add(minNode);
                var minNodeMark = marks[minNode];
                foreach ( var node in listNodes ) {
                    if ( marked.Contains(node) || (!neighboursGetter(node).Contains(minNode)) ) {
                        continue;
                    }
                    var newWeight = minNodeMark + weightGetter(node, minNode);
                    if ( marks[node] > newWeight ) {
                        marks[node] = newWeight;
                        paths[node] = new List<string>(paths[minNode]) { node };
                    }
                }
            }
            return (paths[finishNode], marks[finishNode]);
        }
    }
}
