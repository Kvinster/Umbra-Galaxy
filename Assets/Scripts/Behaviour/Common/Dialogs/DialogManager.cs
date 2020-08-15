using UnityEngine;

using System.Collections.Generic;
using System.Linq;

using STP.Config;
using STP.State;

namespace STP.Behaviour.Common.Dialogs {
    public sealed class DialogManager {
        const string ReadyToCompleteQuestCondition = "ready_to_complete_quest";
        
        readonly DialogController _dialogController;
        readonly QuestsController _questsController;

        DialogInfo             _curDialogInfo;
        List<DialogChoiceInfo> _curDialogChoices;
        
        public string       CurDialogName    { get; private set; }
        public string       CurDialogText    { get; private set; }
        public List<string> CurDialogChoices { get; private set; }

        public bool IsDialogActive => !string.IsNullOrEmpty(CurDialogName);

        public DialogManager(DialogController dialogController, QuestsController questsController) {
            _dialogController = dialogController;
            _questsController = questsController;
        }

        public bool TryStartDialog(string dialogName, params object[] args) {
            if ( !string.IsNullOrEmpty(CurDialogName) ) {
                Debug.LogErrorFormat("Can't start dialog '{0}': dialog '{1}' is already active", dialogName,
                    CurDialogName);
                return false;
            }
            _curDialogInfo = _dialogController.GetDialogInfo(dialogName);
            if ( _curDialogInfo == null ) {
                return false;
            }

            _curDialogChoices = _curDialogInfo.Choices.Where(x => CheckCondition(x.Condition)).ToList();
            
            CurDialogName    = dialogName;
            CurDialogText    = string.Format(_curDialogInfo.Text.Text, args);
            CurDialogChoices = _curDialogChoices.Select(x => string.Format(x.Text.Text, args)).ToList();
            
            return true;
        }
        
        public bool TryFinishDialog(int choiceIndex, out string responseKey) {
            responseKey = null;
            if ( string.IsNullOrEmpty(CurDialogName) ) {
                Debug.LogErrorFormat("Can't finish dialog: none active");
                return false;
            }
            if ( (choiceIndex < 0) || (choiceIndex >= _curDialogInfo.Choices.Count) ) {
                Debug.LogErrorFormat("Invalid choice index '{0}'", choiceIndex);
                return false;
            }
            var choiceInfo = _curDialogChoices[choiceIndex];
            responseKey = choiceInfo.ResponseKey;

            FinishDialog();
            return true;
        }

        public void ForceFinishDialog() {
            if ( !IsDialogActive ) {
                return;
            }
            FinishDialog();
        }

        bool CheckCondition(string condition) {
            if ( string.IsNullOrEmpty(condition) ) {
                return true;
            }
            switch ( condition ) {
                case ReadyToCompleteQuestCondition: {
                    return _questsController.HasReadyToCompleteQuest();
                }
                default: {
                    Debug.LogErrorFormat("Unsupported condition '{0}'", condition);
                    return false;
                }
            }
        }

        void FinishDialog() {
            _curDialogInfo    = null;
            _curDialogChoices = null;
            CurDialogName     = null;
            CurDialogText     = null;
            CurDialogChoices  = null;
        }
    }
}
