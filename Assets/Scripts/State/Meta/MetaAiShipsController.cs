using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.State.Meta {
    public sealed class MetaAiShipsController : BaseStateController {
        public const int MaxAiShips                    = 4;
        public const int MinDaysBetweenAiShipsCreation = 2;
        public const int MinStationaryWait             = 2;
        public const int MaxStationaryWait             = 5;
        
        readonly MetaAiShipsControllerState _state = new MetaAiShipsControllerState();
        
        readonly TimeController _timeController;

        public int LastAiShipCreatedDay {
            get => _state.LastShipCreatedDay;
            set => _state.LastShipCreatedDay = value;
        }

        public MetaAiShipsController(TimeController timeController) {
            _timeController = timeController;
        }

        public List<MetaAiShipState> GetAiShipsStates() {
            var res = new List<MetaAiShipState>();
            foreach ( var pair in _state.ShipStates ) {
                res.Add(pair.Value);
            }
            return res;
        }

        public MetaAiShipState CreateAiShipState() {
            var state = new MetaAiShipState(Guid.NewGuid().ToString());
            return state;
        }

        public bool TryRegisterAiShip(MetaAiShipState state) {
            foreach ( var pair in _state.ShipStates ) {
                if ( pair.Key == state.Id ) {
                    Debug.LogErrorFormat("MetaAiShip with id '{0}' is already registered", state.Id);
                    return false;
                }
            }
            if ( !state.IsValid ) {
                Debug.LogErrorFormat("MetaAiShipState for id '{0}' is invalid", state.Id);
                return false;
            }
            _state.ShipStates.Add(state.Id, state);
            return true;
        }

        public bool TryUnregisterAiShip(string id) {
            if ( !_state.ShipStates.ContainsKey(id) ) {
                Debug.LogErrorFormat("No registered MetaAiShipStates for id '{0}'", id);
                return false;
            }
            _state.ShipStates.Remove(id);
            return true;
        }
    }
}
