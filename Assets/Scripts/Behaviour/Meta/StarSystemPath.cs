using System.Collections.Generic;

namespace STP.Behaviour.Meta {
    public sealed class StarSystemPath {
        public readonly List<string> Path;
        public readonly int          PathLength;

        public string StartStarSystemName  => Path[0];
        public string FinishStarSystemName => Path[Path.Count - 1];

        public StarSystemPath(IEnumerable<string> path, int pathLength) {
            Path       = new List<string>(path);
            PathLength = pathLength;
        }
    }
}
