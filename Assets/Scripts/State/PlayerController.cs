using UnityEngine;

using System;

using STP.Gameplay.Weapon.Common;

namespace STP.State {
    public sealed class PlayerController : BaseStateController {
        static int MaxFuel => PlayerState.MaxFuel;

        readonly PlayerState _state = new PlayerState();

        public PlayerInventory Inventory => _state.Inventory;

        public PlayerShipState CurPlayerShipState => _state.CurShipState;

        public string CurSystemId {
            get => _state.CurSystemId;
            set {
                if ( CurSystemId == value ) {
                    return;
                }
                _state.CurSystemId = value;
                OnCurSystemChanged?.Invoke(CurSystemId);
            }
        }

        public int Fuel {
            get => _state.Fuel;
            set {
                if ( (value < 0) || (value > MaxFuel) ) {
                    Debug.LogWarningFormat("New fuel value is outside the acceptable range: '{0}'", value);
                    value = Mathf.Clamp(value, 0, MaxFuel);
                }
                if ( Fuel == value ) {
                    return;
                }
                _state.Fuel = value;
                OnFuelChanged?.Invoke(Fuel);
            }
        }

        public int Money {
            get => _state.Money;
            set {
                if ( value < 0 ) {
                    Debug.LogWarningFormat("New money value is below zero: '{0}'", value);
                    value = 0;
                }
                if ( Money == value ) {
                    return;
                }
                _state.Money = value;
                OnMoneyChanged?.Invoke(Money);
            }
        }

        public WeaponType CurWeaponType {
            get => _state.CurWeaponType;
            set {
                if ( value == WeaponType.Unknown ) {
                    Debug.LogError("Trying to set CurWeaponType to Unknown");
                    return;
                }
                if ( CurWeaponType == value ) {
                    return;
                }
                _state.CurWeaponType = value;
                OnWeaponChanged?.Invoke(CurWeaponType);
            }
        }

        public float ShipHp {
            get => _state.CurShipState.Hp;
            set {
                if ( Mathf.Approximately(ShipHp, value) ) {
                    return;
                }
                // TODO: clamp
                _state.CurShipState.Hp = value;
                OnShipHpChanged?.Invoke(value);
            }
        }

        public event Action<string>     OnCurSystemChanged;
        public event Action<int>        OnFuelChanged;
        public event Action<int>        OnMoneyChanged;
        public event Action<WeaponType> OnWeaponChanged;
        public event Action<float>      OnShipHpChanged;

        public override void Init() {
            CurSystemId   = "bd6537e4a0b08a2449e4d595f48ab96e"; // Cradle
            Fuel          = MaxFuel;
            Money         = 0;
            CurWeaponType = WeaponType.Gun;
        }
    }
}
