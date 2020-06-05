﻿using UnityEngine;

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

        int _fuel = 0;
        public int Fuel {
            get => _fuel;
            set {
                if ( (value < 0) || (value > MaxFuel) ) {
                    Debug.LogWarningFormat("New fuel value is outside the acceptable range: '{0}'", value);
                    value = Mathf.Clamp(value, 0, MaxFuel);
                }
                if ( _fuel == value ) {
                    return;
                }
                _fuel = value;
                OnFuelChanged?.Invoke(_fuel);
            }
        }

        int _money = 0;
        public int Money {
            get => _money;
            set {
                if ( value < 0 ) {
                    Debug.LogWarningFormat("New money value is below zero: '{0}'", value);
                    value = 0;
                }
                if ( _money == value ) {
                    return;
                }
                _money = value;
                OnMoneyChanged?.Invoke(_money);
            }
        }

        public event Action OnInventoryChanged;
        public event Action<int> OnFuelChanged;
        public event Action<int> OnMoneyChanged;

        public Dictionary<string, int>.Enumerator GetInventoryEnumerator() {
            return _inventory.GetEnumerator();
        }

        public int GetInventoryItemAmount(string itemName) {
            return _inventory.ContainsKey(itemName) ? _inventory[itemName] : -1;
        }

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
