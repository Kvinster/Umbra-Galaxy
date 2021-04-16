using UnityEngine;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core.Generators {
	public interface ILevelGeneratorImpl {
		Rect AreaRect { get; }

		UniTask GenerateLevel();
	}
}
