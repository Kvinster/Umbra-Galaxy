using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.Core.Enemy.GeneratorEditor;
using STP.Behaviour.Core.Generators.Regular;
using STP.Behaviour.Starter;
using STP.Config;
using STP.Core;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;

namespace STP.Behaviour.Core.Generators {
	public sealed class LevelGenerator : GameComponent {
		[NotNull] public ChunkCreator Creator;
		[NotNull] public Transform    BordersRoot;

		Transform         _levelObjectsRoot;
		CoreStarter       _coreStarter;
		PrefabsController _prefabsController;

		BaseLevelInfo _curLevelInfo;

		public LevelGenerator Init(Transform levelObjectsRoot, CoreStarter coreStarter, PrefabsController prefabsController,
			LevelController levelController) {
			_levelObjectsRoot  = levelObjectsRoot;
			_coreStarter       = coreStarter;
			_prefabsController = prefabsController;

			_curLevelInfo = levelController.GetCurLevelConfig();

			return this;
		}

		public async UniTask GenerateLevel() {
			await CreateImplementation().GenerateLevel();
		}

		ILevelGeneratorImpl CreateImplementation() {
			Assert.IsNotNull(_curLevelInfo);
			switch ( _curLevelInfo ) {
				case RegularLevelInfo regularLevelInfo: {
					return new RegularLevelGeneratorImpl(regularLevelInfo, _levelObjectsRoot, BordersRoot, Creator,
						_coreStarter, _prefabsController);
				}
				default: {
					Debug.LogErrorFormat("{0}.{1}: unsupported level info type '{2}'", nameof(LevelGenerator),
						nameof(CreateImplementation), _curLevelInfo.GetType().Name);
					return null;
				}
			}
		}
	}
}