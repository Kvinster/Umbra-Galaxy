using UnityEngine;
using UnityEngine.UI;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.View {
    public class HpBar : GameBehaviour {
        [NotNull]
        public Image CurHp;
        [NotNull]
        public Image BarBase;
        public bool  HideIfFull;
        
        float _fullBarSize;
        

        public void Init() {
            _fullBarSize = BarBase.rectTransform.sizeDelta.x;
            UpdateBar(1f);
        }
        
        //leftHp - parameter between 0 and 1. 0 - no Hp. 1 - full Hp
        public void UpdateBar(float leftHp) {
            var x = Mathf.Clamp01(leftHp) * _fullBarSize;
            var y = CurHp.rectTransform.sizeDelta.y;
            CurHp.rectTransform.sizeDelta = new Vector2(x, y);
            BarBase.gameObject.SetActive(!HideIfFull || (1f - leftHp > float.Epsilon));
        }
    }
}