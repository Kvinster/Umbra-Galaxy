using System.Collections.Generic;
using System.Linq;

using XNode;

namespace STP.Config {
	public class LevelNode : Node {
		public BaseLevelInfo Config;

		[Input]  public List<LevelNode> NextLevelsInput;
		[Input]  public List<LevelNode> OptionalLevelsInput;
		[Output] public LevelNode       ThisNode;

		public List<LevelNode>  NextLevels     => GetInputValues<LevelNode>("NextLevelsInput").ToList();
		public List<LevelNode>  OptionalLevels => GetInputValues<LevelNode>("OptionalLevelsInput").ToList();

		// GetValue should be overridden to return a value for any specified output port
		public override object GetValue(NodePort port) {
			return this;
		}
	}
}