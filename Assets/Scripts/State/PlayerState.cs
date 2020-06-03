using UnityEngine;

using System;
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

        readonly Dictionary<string, int> _inventory = new Dictionary<string, int>();

        // TODO: set for the sake of testing, revert
        int _fuel = 50;
        public int Fuel {
            get => _fuel;
            set {
                if ( _fuel == value ) {
                    return;
                }
                if ( (value < 0) || (value > MaxFuel) ) {
                    Debug.LogWarningFormat("New fuel value is outside the acceptable range: '{0}'", value);
                    value = Mathf.Clamp(value, 0, MaxFuel);
                }
                _fuel = value;
                OnFuelChanged?.Invoke(_fuel);
            }
        }

        public event Action OnInventoryChanged;
        public event Action<int> OnFuelChanged;

        public bool HasInInventory(string itemName, int amount = 1) {
            return _inventory.ContainsKey(itemName) && (_inventory[itemName] >= amount);
        }

        public bool TryTakeFromInventory(string itemName, int amount = 1) {
            if ( HasInInventory(itemName, amount) ) {
                _inventory[itemName] -= amount;
                if ( _inventory[itemName] <= 0 ) {
                    _inventory.Remove(itemName);
                }
                OnInventoryChanged?.Invoke();
                return true;
            }
            return false;
        }

        public void AddToInventory(string itemName, int amount) {
            if ( _inventory.ContainsKey(itemName) ) {
                _inventory[itemName] += amount;
            } else {
                _inventory.Add(itemName, amount);
            }
            OnInventoryChanged?.Invoke();
        }
    }
}
