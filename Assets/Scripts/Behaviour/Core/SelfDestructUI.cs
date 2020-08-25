using STP.Behaviour.Starter;
using STP.Gameplay;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using TMPro;

namespace STP.Behaviour.Core {
    public class SelfDestructUI : CoreComponent {
        [NotNull] public TMP_Text SelfDestructionText;
    
        SelfDestructEngine _engine;
        
        Timer Timer => _engine.Timer;

        public override void Init(CoreStarter starter) {
            _engine = starter.CoreManager.SelfDestructEngine;
            _engine.OnStart += OnStartSelfDestruction;
            _engine.OnStop  += OnStopSelfDestruction;
            gameObject.SetActive(false);
        }

        void OnDestroy() {
            _engine.OnStart -= OnStartSelfDestruction;
            _engine.OnStop  -= OnStopSelfDestruction;
        }

        void OnStartSelfDestruction() {
            gameObject.SetActive(true);
        }

        void OnStopSelfDestruction() {
            gameObject.SetActive(false);
        }
        
        void Update() {
            SelfDestructionText.text = HRTime.ConvertToSMString(Timer.TimeLeft);
        }
    }
}