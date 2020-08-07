using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using STP.Common;
using STP.State;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Meta.UI {
    public sealed class GameOverScreen : GameBehaviour {
        const string CradleConqueredText =
            "Bad Ending\nThe Cradle of Humanity has been destroyed. The light of humanity has been consumed by Darkness";

        const string DictatorshipTextTemplate =
            "Dictatorship Ending\nFaction {0} has gathered enought uber artifacts to create the ultimate weapon. " +
            "With no competition or threats, it quickly got rid of both the Darkness and two other factions and soon " +
            "came to rule the humanity with an iron fist"; // 3+-N-N

        const string AllianceTextTemplate =
            "Alliance Ending\nTogether {0} and {1} factions developed the weapon to destroy Darkness. After the war was" +
            " over, the united {0}&{1} Court has proclaimed that {2}'s efforts to save the humanity were deemed " +
            "traitorously insufficient and weak. The members of {2} faction were prosecuted for the assumed betrayal of" +
            " the mankind, its territory and resources divided between the now ruling factions."; // 2-2-1, 2-2-0

        const string UnityText =
            "Unity Ending\nUnited stronger than ever, the three factions, all of humanity together created the ultimate " +
            "weapon that granted mankind a way to oppose Darkness. What first was a temporary scientific alliance grew" +
            " to unify all of humanity under the just rule of the Council of United Mankind. Soon after the Darkness " +
            "was destroyed, the humanity stepped into its second Golden Age."; // 2-2-2

        const string WarTextTemplate =
            "War Enging\nWith all the uber technology equally divided between {0} and {1} factions, {2} faction quickly" +
            " lost all weight on the political arena and soon vanished. The remaining factions started a ruthless and " +
            "violent technology race, juicing every last bit of the alien tech. The Darkness was eliminated quickly, " +
            "but soon after that the humanity followed suit in the flames of the Last War"; // 3-3-0
        
        [NotNull] public TMP_Text Text;

        MetaUiManager      _owner;
        ProgressController _progressController;

        void OnDestroy() {
            _progressController.OnGameFinished -= OnGameFinished;
        }

        public void CommonInit(MetaUiManager owner, ProgressController progressController) {
            _owner              = owner;
            _progressController = progressController;

            _progressController.OnGameFinished += OnGameFinished;

            gameObject.SetActive(true);
        }

        void OnGameFinished(bool win) {
            if ( win ) {
                var artifactsValues = _progressController.UberArtifacts.Values.ToList();
                artifactsValues.Sort();
                var factions = new List<Faction>(artifactsValues.Count);
                foreach ( var artifactValue in artifactsValues ) {
                    foreach ( var faction in (Faction[]) Enum.GetValues(typeof(Faction)) ) {
                        if ( (faction == Faction.Unknown) || factions.Contains(faction) ) {
                            continue;
                        }
                        if ( _progressController.UberArtifacts[faction] == artifactValue ) {
                            factions.Add(faction);
                            break;
                        }
                    }
                }
                if ( (artifactsValues[0] == 0) && (artifactsValues[1] == 3) && (artifactsValues[2] == 3) ) {
                    Text.text = string.Format(WarTextTemplate, factions[1], factions[2], factions[0]);
                } else if ( (artifactsValues[0] == 2) && (artifactsValues[1] == 2) && (artifactsValues[2] == 2) ) {
                    Text.text = UnityText;
                } else if ( (artifactsValues[0] < 2) && (artifactsValues[1] == 2) && (artifactsValues[2] == 2) ) {
                    Text.text = string.Format(AllianceTextTemplate, factions[1], factions[2], factions[0]);
                } else if ( (artifactsValues[0] < 3) && (artifactsValues[1] < 3) && (artifactsValues[2] >= 3) ) {
                    Text.text = string.Format(DictatorshipTextTemplate, factions[2]);
                } else {
                    Debug.LogErrorFormat("Unexpected ending: {0}-{1}-{2}", artifactsValues[0], artifactsValues[1],
                        artifactsValues[2]);
                }
            } else {
                Text.text = CradleConqueredText;
            }
            _owner.ShowGameOverScreen();
        }
    }
}
