using UnityEngine;
using UnityEngine.UI;

namespace STP.View {
    public class HpBar : MonoBehaviour {
        public Image CurHp;
        public Image FullBar;

        float _fullBarSize;

        public void Init() {
            _fullBarSize = FullBar.rectTransform.sizeDelta.x;
        }
        
        //leftHp - parameter between 0 and 1. 0 - no Hp. 1 - full Hp
        public void UpdateBar(float leftHp) {
            var x    = Mathf.Clamp01(leftHp) * _fullBarSize;
            var y    = CurHp.rectTransform.sizeDelta.y;
            CurHp.rectTransform.sizeDelta = new Vector2(x, y);
        }
    }
}