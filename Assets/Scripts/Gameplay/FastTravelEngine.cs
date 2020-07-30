using System;

using STP.Utils;

namespace STP.Gameplay {
    public enum EngineState {
        IDLE,
        CHARGING,
        CHARGED
    }
    
    public class FastTravelEngine {
        public Timer Timer = new Timer();
        
        float  _chargingTime;
        Action _chargedCallback;
        
        public EngineState State {get; private set;}

        public void Init(float chargingTime) {
            _chargingTime = chargingTime;
        }
        
        public void TryStartEngine(Action chargedCallback) {
            if ( State != EngineState.IDLE ) {
                return;
            } 
            _chargedCallback = chargedCallback;
            Timer.Start(_chargingTime);
            State = EngineState.CHARGING;
        }

        public void UpdateEngineState(float deltaTime) {
            if ( ( State != EngineState.CHARGING ) ||  !Timer.Tick(deltaTime) ) {
                return;
            }
            State = EngineState.IDLE;
            _chargedCallback();
        }

        public void StopEngine() {
            State = EngineState.IDLE;
            Timer.Stop();
        }
    }
}