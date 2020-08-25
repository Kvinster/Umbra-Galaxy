using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using STP.Common;
using STP.Common.Windows;
using STP.State;
using STP.State.Meta;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Meta.UI.QuestsWindow {
    public sealed class QuestsWindow : BaseWindow {
        [NotNull] public GameObject NoActiveQuestsRoot;
        [NotNull] public GameObject ActiveQuestsRoot;
        [NotNull] public Transform  ActiveQuestsParent;
        [Space]
        [NotNull] public GameObject QuestBlockPrefab;

        readonly List<QuestsWindowQuestBlock> _questBlocksPool = new List<QuestsWindowQuestBlock>();

        public void Init(QuestsController questsController, StarSystemsController starSystemsController) {
            // TODO: show Finished quests
            var questStates = questsController.GetQuestStates().Where(x => x.Status == QuestStatus.Started).ToList();
            if ( questStates.Count > 0 ) {
                NoActiveQuestsRoot.SetActive(false);
                ActiveQuestsRoot.SetActive(true);
                foreach ( var questState in questStates ) {
                    var questBlock = GetFreeQuestBlock();
                    if ( questBlock ) {
                        questBlock.Init(questState, starSystemsController);
                        questBlock.gameObject.SetActive(true);
                    }
                }
                foreach ( var questBlock in _questBlocksPool ) {
                    if ( !questBlock.IsActive ) {
                        questBlock.gameObject.SetActive(false);
                    }
                }
            } else {
                NoActiveQuestsRoot.SetActive(true);
                ActiveQuestsRoot.SetActive(false);
            }
        }

        protected override void Deinit() {
            foreach ( var questBlock in _questBlocksPool ) {
                questBlock.Deinit();
                questBlock.gameObject.SetActive(false);
            }
        }

        QuestsWindowQuestBlock GetFreeQuestBlock() {
            foreach ( var questBlock in _questBlocksPool ) {
                if ( !questBlock.IsActive ) {
                    return questBlock;
                }
            }
            {
                if ( !QuestBlockPrefab ) {
                    Debug.LogError("QuestBlockPrefab is null");
                    return null;
                }
                var questBlockGo = Instantiate(QuestBlockPrefab, ActiveQuestsParent, false);
                var questBlock = questBlockGo.GetComponent<QuestsWindowQuestBlock>();
                if ( !questBlock ) {
                    Debug.LogError("No QuestsWindowQuestBlock component on quest block instance");
                    return null;
                }
                _questBlocksPool.Add(questBlock);
                return questBlock;
            }
        }
    }
}
