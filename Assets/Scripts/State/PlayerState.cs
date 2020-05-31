using UnityEngine;

using System.Collections.Generic;

namespace STP.State {
    public class PlayerState {
        public const int MaxFuel = 50;
        
        static PlayerState _instance;
        public static PlayerState Instance {
            get {
                if ( _instance == null ) {
                    _instance = new PlayerState();
                }
                
                return _instance;
            }
        }

        public readonly Dictionary<string, int> Inventory = new Dictionary<string, int>();

        // TODO: set for the sake of testing, revert
        int _fuel = 50;
        public int Fuel {
            get => _fuel;
            set {
                if ( (value < 0) || (value > MaxFuel) ) {
                    Debug.LogWarningFormat("New fuel value is outside the acceptable range: '{0}'", value);
                    value = Mathf.Clamp(value, 0, MaxFuel);
                }
                _fuel = value;
            }
        }
    }
}
