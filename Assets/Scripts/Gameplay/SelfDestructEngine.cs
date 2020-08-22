using System;

using STP.Utils;

namespace STP.Gameplay {
    public class SelfDestructEngine {
        const float StartSelfDestructionTime = 25f;
        
        const float DamageAmount = float.MaxValue;
        
        public readonly Timer Timer = new Timer();

        public event Action OnStart;
        public event Action OnStop;
        
        bool _starting;
        
        IDestructable _itemToDestruct;
        
        public bool IsActive { get; private set; }

        public void Init(IDestructable itemToDestruct) {
            _itemToDestruct = itemToDestruct;
        } 
        
        public void StartSelfDestruction() {
            Timer.Start(StartSelfDestructionTime);
            IsActive  = true;
            OnStart?.Invoke();
        }

        public void StopSelfDestruction() {
            Timer.Stop();
            IsActive = false;
            OnStop?.Invoke();
        }

        public void UpdateSelfDestructionTimers(float passedTime) {
            if ( !IsActive ) {
                return;
            }
            if ( Timer.Tick(passedTime) ) {
                _itemToDestruct.GetDamage(DamageAmount);
                StopSelfDestruction();
            }
        }
    }
}