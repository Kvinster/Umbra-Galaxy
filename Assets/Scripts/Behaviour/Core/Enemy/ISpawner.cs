namespace STP.Behaviour.Core.Enemy {
	public interface ISpawner {
		void Init(CoreSpawnHelper spawnHelper);
		void Spawn();
	}
}