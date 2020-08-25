﻿using System.Collections.Generic;

namespace STP.State.Meta {
    public sealed class MetaAiShipsControllerState {
        public int LastShipCreatedDay = -1;
        
        public Dictionary<string, MetaAiShipState> ShipStates = new Dictionary<string, MetaAiShipState>();
    }
}