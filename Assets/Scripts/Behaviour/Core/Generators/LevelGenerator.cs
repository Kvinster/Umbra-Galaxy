using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.Core.Generators.Boss;
using STP.Behaviour.Core.Generators.Regular;
using STP.Behaviour.Starter;
using STP.Config;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core.Generators {
	public sealed class LevelGenerator {
		readonly CoreStarter _coreStarter;

		readonly ILevelGeneratorImpl _impl;

		public Rect AreaRect => _impl?.AreaRect ?? default;

		public LevelGenerator(CoreStarter coreStarter) {
			_coreStarter = coreStarter;

			_impl = CreateImplementation(coreStarter.LevelController.GetCurLevelConfig());
		}

		public async UniTask GenerateLevel() {
			await (_impl?.GenerateLevel() ?? default);
		}

		ILevelGeneratorImpl CreateImplementation(BaseLevelInfo curLevelInfo) {
			Assert.IsNotNull(curLevelInfo);
			switch ( curLevelInfo ) {
				case RegularLevelInfo regularLevelInfo: {
					return new RegularLevelGeneratorImpl(regularLevelInfo, _coreStarter.LevelObjectsRoot,
						_coreStarter.BordersRoot, _coreStarter.Player, _coreStarter.PrefabsController);
				}
				case BossLevelInfo bossLevelInfo: {
					return new BoosLevelGeneratorImpl(bossLevelInfo, _coreStarter.LevelObjectsRoot,
						_coreStarter.BordersRoot, _coreStarter.Player);
				}
				default: {
					Debug.LogErrorFormat("{0}.{1}: unsupported level info type '{2}'", nameof(LevelGenerator),
						nameof(CreateImplementation), curLevelInfo.GetType().Name);
					return null;
				}
			}
		}
	}
}
