using UnityEngine;

namespace STP.State {
    public sealed class MetaAiShipState {
        public readonly string Id;

        public MetaAiShipMode CurMode      = MetaAiShipMode.Moving;
        public string         CurSystemId  = string.Empty;
        public string         DestSystemId = string.Empty;
        public int            CurDay       = -1;
        public int            DestDay      = -1;

        public bool IsValid {
            get {
                if ( string.IsNullOrEmpty(Id) ) {
                    return false;
                }
                switch ( CurMode ) {
                    case MetaAiShipMode.Moving: {
                        if ( string.IsNullOrEmpty(CurSystemId) || string.IsNullOrEmpty(DestSystemId) || (CurDay < 0) ||
                             (DestDay <= 0) || (CurDay >= DestDay) ) {
                            return false;
                        }
                        break;
                    }
                    case MetaAiShipMode.Stationary: {
                        if ( string.IsNullOrEmpty(CurSystemId) || (DestDay <= 0) ) {
                            return false;
                        }
                        break;
                    }
                    default: {
                        Debug.LogErrorFormat("Unsupported MetaAiShipMode '{0}'", CurMode.ToString());
                        return false;
                    }
                }
                return true;
            }
        }

        public MetaAiShipState(string id) {
            Id = id;
        }
    }
}
