﻿using UnityEngine;

using System;

using STP.Gameplay.Weapon.Common;

namespace STP.State {
    public class PlayerState {
        public const int MaxFuel = 100;
        
        static PlayerState _instance;
        public static PlayerState Instance {
            get {
                if ( _instance == null ) {
                    _instance = new PlayerState();
                }
                
                return _instance;
            }
        }
        
        public readonly PlayerInventory Inventory = new PlayerInventory();
        
        string _curSystemId = "bd6537e4a0b08a2449e4d595f48ab96e"; // Cradle

        public string CurSystemId {
            get => _curSystemId;
            set {
                if ( _curSystemId == value ) {
                    return;
                }
                _curSystemId = value;
                OnCurSystemChanged?.Invoke(_curSystemId);
            }
        }
        
        int _fuel = MaxFuel;
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

        WeaponType _curWeaponType = WeaponType.Gun;
        public WeaponType CurWeaponType {
            get => _curWeaponType;
            set {
                if ( value == WeaponType.Unknown ) {
                    Debug.LogError("Trying to set CurWeaponType to Unknown");
                    return;
                }
                if ( _curWeaponType == value ) {
                    return;
                }
                _curWeaponType = value;
                OnWeaponChanged?.Invoke(_curWeaponType);
            }
        }
        
        public event Action<string>     OnCurSystemChanged;
        public event Action<int>        OnFuelChanged;
        public event Action<int>        OnMoneyChanged;
        public event Action<WeaponType> OnWeaponChanged;
    }
}
