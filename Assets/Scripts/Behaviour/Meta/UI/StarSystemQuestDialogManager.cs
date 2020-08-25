using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Behaviour.Common.Dialogs;
using STP.Common;
using STP.State;
using STP.State.Meta;
using STP.State.QuestStates;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Meta.UI {
    public sealed class StarSystemQuestDialogManager : GameComponent {
        const string DialogNamePrefix         = "quest_screen|";
        const string InitDialogName           = DialogNamePrefix + "init";
        const string AcceptedQuestDialogName  = DialogNamePrefix + "accepted_quest";
        const string DeniedQuestDialogName    = DialogNamePrefix + "denied_quest";
        const string CompletedQuestDialogName = DialogNamePrefix + "completed_quest";
        const string ProposeDialogNamePrefix  = DialogNamePrefix + "propose_";
        const string NoQuestDialogName        = ProposeDialogNamePrefix + "no_quests";

        const string AskQuestResponseKey      = "ask_quest";
        const string AcceptQuestResponseKey   = "accept_quest";
        const string DenyQuestResponseKey     = "deny_quest";
        const string CompleteQuestResponseKey = "complete_quest";
        const string ResetResponseKey         = "reset";
        const string ExitResponseKey          = "exit";

        static readonly Dictionary<QuestType, string> QuestTypeToPostfix = new Dictionary<QuestType, string> {
            { QuestType.GatherResource, "gather_resources" },
            { QuestType.Escort,         "escort" },
            { QuestType.ReclaimSystem,  "reclaim_system" },
            { QuestType.DefendSystem,   "defend_system" },
            { QuestType.Delivery,       "delivery" },
        };
        
        [NotNull] public StarSystemQuestDialogView DialogView;

        Action                _hide;
        DialogManager         _dialogManager;
        QuestHelper           _questHelper;
        StarSystemsController _starSystemsController;
        PlayerController      _playerController;

        BaseQuestState _proposedQuestState;

        public void Init(Action hide, QuestHelper questHelper, DialogController dialogController,
            StarSystemsController starSystemsController, PlayerController playerController) {
            _hide                  = hide;
            _dialogManager         = new DialogManager(questHelper, dialogController);
            _starSystemsController = starSystemsController;
            _questHelper           = questHelper;
            _playerController      = playerController;
            
            DialogView.CommonInit();
            DialogView.OnChoiceClick += OnChoiceClick;
        }
        
        public void Deinit() {
            _dialogManager.ForceFinishDialog();
            DialogView.Deinit();
            
            _hide             = null;
            _dialogManager    = null;
            _questHelper      = null;
            _playerController = null;

            _proposedQuestState = null;
        }

        public void Show() {
            if ( _dialogManager.TryStartDialog(InitDialogName) ) {
                UpdateView();
            } else {
                Debug.LogError("Can't start initial dialog");
            }
        }

        void Hide() {
            _dialogManager.ForceFinishDialog();
            _hide?.Invoke();
        }
        
        void OnChoiceClick(int choiceIndex) {
            if ( _dialogManager.TryFinishDialog(choiceIndex, out var responseKey) ) {
                if ( TryReact(responseKey) && _dialogManager.IsDialogActive ) {
                    UpdateView();
                }
            } else {
                Debug.LogErrorFormat("Unsupported scenario: can't finish dialog '{0}' by choice '{1}'",
                    _dialogManager.CurDialogName, choiceIndex);
            }
        }

        bool TryReact(string responseKey) {
            switch ( responseKey ) {
                case AskQuestResponseKey: {
                    if ( _questHelper.TryCreateRandomQuest(_playerController.CurSystemId, out var questState) &&
                         TryStartQuestSuggestDialog(questState) ) {
                        return true;
                    }
                    return _dialogManager.TryStartDialog(NoQuestDialogName);
                }
                case AcceptQuestResponseKey: {
                    if ( _proposedQuestState == null ) {
                        Debug.LogError("Can't accept quest: none proposed");
                        return false;
                    }
                    if ( _questHelper.TryStartQuest(_proposedQuestState) ) {
                        _proposedQuestState = null;
                        return _dialogManager.TryStartDialog(AcceptedQuestDialogName);
                    }
                    Debug.LogErrorFormat("Can't start quest '{0}'", _proposedQuestState);
                    return false;
                }
                case DenyQuestResponseKey: {
                    if ( _proposedQuestState == null ) {
                        Debug.LogError("Quest denied, but there was none proposed");
                        return false;
                    }
                    _proposedQuestState = null;
                    return _dialogManager.TryStartDialog(DeniedQuestDialogName);
                }
                case CompleteQuestResponseKey: {
                    var questState = _questHelper.GetReadyToCompleteQuest();
                    if ( questState == null ) {
                        Debug.LogError("No ready to complete quest");
                        return false;
                    }
                    if ( _questHelper.TryCompleteQuest(questState) ) {
                        return _dialogManager.TryStartDialog(CompletedQuestDialogName);
                    }
                    Debug.LogErrorFormat("Can't complete quest '{0}'", questState);
                    return false;
                }
                case ResetResponseKey: {
                    return _dialogManager.TryStartDialog(InitDialogName);
                }
                case ExitResponseKey: {
                    Hide();
                    return true;
                }
                default: {
                    Debug.LogErrorFormat("Unsupported response key '{0}'", responseKey);
                    return false;
                }
            }
        }

        bool TryStartQuestSuggestDialog(BaseQuestState questState) {
            _proposedQuestState = questState;
            if ( QuestTypeToPostfix.TryGetValue(questState.QuestType, out var questPostfix) ) {
                return _dialogManager.TryStartDialog(ProposeDialogNamePrefix + questPostfix, GetQuestArgs(questState));
            }
            Debug.LogErrorFormat("Unsupported quest type '{0}'", questState.QuestType.ToString());
            return false;
        }

        object[] GetQuestArgs(BaseQuestState baseQuestState) {
            if ( baseQuestState == null ) {
                Debug.LogError("Quest state is null");
                return null;
            }
            switch ( baseQuestState.QuestType ) {
                case QuestType.GatherResource when baseQuestState is GatherResourcesQuestState questState: {
                    return new object[] {
                        questState.ResourceAmount, questState.ResourceType, questState.ExpirationDay,
                        questState.RewardInfo.Money
                    };
                }
                case QuestType.Escort: {
                    return new object[] {
                        _starSystemsController.GetStarSystemName(baseQuestState.DestSystemId),
                        baseQuestState.ExpirationDay, baseQuestState.RewardInfo.Money
                    };
                }
                case QuestType.ReclaimSystem: {
                    return new object[] {
                        _starSystemsController.GetStarSystemName(baseQuestState.DestSystemId),
                        baseQuestState.ExpirationDay, baseQuestState.RewardInfo.Money
                    };
                }
                case QuestType.DefendSystem: {
                    return new object[] {
                        _starSystemsController.GetStarSystemName(baseQuestState.DestSystemId),
                        baseQuestState.ExpirationDay,
                        baseQuestState.RewardInfo.Money
                    };
                }
                case QuestType.Delivery: {
                    return new object[] {
                        _starSystemsController.GetStarSystemName(baseQuestState.DestSystemId),
                        baseQuestState.ExpirationDay, baseQuestState.RewardInfo.Money
                    };
                }
                default: {
                    Debug.LogErrorFormat("Unsupported quest type '{0}'", baseQuestState.QuestType);
                    return null;
                }
            }
        } 

        void UpdateView() {
            if ( !_dialogManager.IsDialogActive ) {
                Debug.LogError("Can't update dialog view: no dialog active");
                return;
            }
            DialogView.UpdateView(_dialogManager.CurDialogText, _dialogManager.CurDialogChoices);
        }
    }
}
