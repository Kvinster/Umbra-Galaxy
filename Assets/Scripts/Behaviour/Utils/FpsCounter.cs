using UnityEngine;

using TMPro;

namespace STP.Behaviour.Common {
    public class FpsCounter : MonoBehaviour {
        public TMP_Text  _fpsText;
        public float     _hudRefreshRate = 1f;

        float _timer;
        void Update() {
            if ( !(Time.unscaledTime > _timer) ) {
                return;
            }
            var fps = (int)(1f / Time.unscaledDeltaTime);
            _fpsText.text = $"{fps.ToString()} FPS";
            _timer = Time.unscaledTime + _hudRefreshRate;
        }
    }
}