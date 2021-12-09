namespace STP.Behaviour.Core {
	public sealed class PlayerBullet : Bullet {
		public override bool NeedToDestroy => base.NeedToDestroy || !IsVisible;
	}
}
