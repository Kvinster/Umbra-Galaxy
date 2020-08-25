using UnityEngine;

using System;
using System.Collections.Generic;

using STP.Events;
using STP.Gameplay;
using STP.Utils.Events;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
    public class EscortLevelWrapper : BaseLevelWrapper {
        [Serializable]
        public class ShipInfo {
            public RoutedShip Ship;
            [HideInInspector]
            public bool       ReachedEnd;
        }

        [NotNullOrEmpty] public List<ShipInfo> Ships;

        void Start() {
            foreach ( var shipInfo in Ships ) {
                shipInfo.Ship.OnShipDestroyed += OnShipDestroyed;
                shipInfo.Ship.ReachedRouteEnd += OnReachedEnd;
            }
        }

        void OnDestroy() {
            foreach ( var shipInfo in Ships ) {
                RemoveCallbacks(shipInfo.Ship);
            }
        }

        void OnReachedEnd(RoutedShip ship) {
            var shipInfo = Ships.Find((x)=>(x.Ship==ship));
            if ( !shipInfo.Ship ) {
                Debug.LogError($"Can't find ship {ship.name} in level ship collection");
                return;
            }
            shipInfo.ReachedEnd = true;
            foreach ( var shipElement in Ships ) {
                if ( !shipElement.ReachedEnd ) {
                    return;
                }
            }
            LevelQuestState = LevelQuestState.Completed;
            EventManager.Fire(new QuestCompleted());
        }

        void OnShipDestroyed(BaseShip ship) {
            Ships.RemoveAll((x)=> (x.Ship == ship));
            RemoveCallbacks(ship as RoutedShip);
            if ( Ships.Count == 0 ) {
                LevelQuestState = LevelQuestState.Failed;
                EventManager.Fire(new QuestFailed());
            }
        }

        void RemoveCallbacks(RoutedShip ship) {
            ship.OnShipDestroyed -= OnShipDestroyed;
            ship.ReachedRouteEnd -= OnReachedEnd;
        }
    }
}