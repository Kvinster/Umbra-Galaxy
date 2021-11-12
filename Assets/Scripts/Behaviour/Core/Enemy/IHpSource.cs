using STP.Core;

namespace STP.Behaviour.Core.Enemy {
	public interface IHpSource {
		HpSystem HpSystemComponent { get; }
	}
}