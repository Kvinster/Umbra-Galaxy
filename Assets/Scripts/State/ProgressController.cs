using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Common;

namespace STP.State {
    public sealed class ProgressController {
        const int    GameFinishingArtifactsAmount = 3;
        const string CradleId                     = "bd6537e4a0b08a2449e4d595f48ab96e";

        static string UberArtifactName => ItemNames.MetaItem;
        
        static ProgressController _instance;
        public static ProgressController Instance => _instance ?? (_instance = new ProgressController());

        readonly ProgressControllerState _state = new ProgressControllerState();

        public bool IsActive { get; private set; } = true;

        public Dictionary<Faction, int> UberArtifacts => _state.UberArtifacts;

        public event Action<bool> OnGameFinished;

        public void OnSellItemToFaction(string artifactName, int artifactsAmount, Faction buyerFaction) {
            if ( artifactName != UberArtifactName ) {
                return;
            }
            if ( artifactsAmount < 0 ) {
                Debug.LogErrorFormat("Invalid artifacts amount: '{0}'", artifactsAmount);
                return;
            }
            if ( artifactsAmount == 0 ) {
                return;
            }
            if ( UberArtifacts.ContainsKey(buyerFaction) ) {
                UberArtifacts[buyerFaction] += artifactsAmount;
                if ( UberArtifacts[buyerFaction] >= GameFinishingArtifactsAmount ) {
                    FinishGame(true);
                }
            } else {
                Debug.LogErrorFormat("Unsupported faction '{0}'", buyerFaction.ToString());
            }
        }

        public void OnStarSystemCaptured(string starSystemId) {
            if ( starSystemId == CradleId ) {
                FinishGame(false);
            }
        }

        void FinishGame(bool win) {
            IsActive = false;
            OnGameFinished?.Invoke(win);
        }
    }
}
