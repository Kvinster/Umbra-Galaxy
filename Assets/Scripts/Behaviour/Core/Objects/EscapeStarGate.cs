using UnityEngine;

using STP.Behaviour.Starter;
using STP.Gameplay;
using STP.Utils;

namespace STP.Behaviour.Core.Objects {
    public sealed class EscapeStarGate : CoreComponent {
        [Range(0f, 20f)]
        public float ExitTime = 3f;
        [Space]
        public SpriteRenderer CenterSpriteRenderer;

        CoreManager _coreManager;

        readonly Timer _timer = new Timer();

        PlayerShip _playerShip;

        float Progress => _timer.NormalizedProgress;

        bool IsActive => _playerShip;

        void Update() {
            if ( IsActive ) {
                if ( _timer.DeltaTick() ) {
                    _coreManager.GoToMeta();
                    CenterSpriteRenderer.color = Color.white;
                } else {
                    CenterSpriteRenderer.color = new Color(1f, 1f, 1f, Progress);
                }
            }
        }

        public override void Init(CoreStarter starter) {
            _coreManager = starter.CoreManager;

            _timer.Start(ExitTime);

            CenterSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        }

        void OnTriggerEnter2D(Collider2D other) {
            var playerShip = other.GetComponent<PlayerShip>();
            if ( playerShip ) {
                _playerShip = playerShip;
                _timer.Reset();
            }
        }

        void OnTriggerExit2D(Collider2D other) {
            if ( _playerShip && (other.gameObject == _playerShip.gameObject) ) {
                _playerShip = null;
            }

            CenterSpriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}
