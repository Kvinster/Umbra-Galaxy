using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.Gameplay;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core.Objects {
    public sealed class DefendStarGate : BaseCoreComponent, IDestructable {
        public int StartHp = 100;
        [NotNull]
        public SpriteRenderer SpriteRenderer;
        [NotNull]
        public HpBar HpBar;

        float _curHp;
        float CurHp {
            get => _curHp;
            set {
                _curHp = Mathf.Max(value, 0);
                HpBar.UpdateBar(_curHp / StartHp);
            }
        }

        bool IsDestroyed => Mathf.Approximately(CurHp, 0f);

        public event Action OnDestroyed;

        protected override void InitInternal(CoreStarter starter) {
            HpBar.Init();
            CurHp = StartHp;
        }

        public void GetDamage(float damageAmount = 1) {
            if ( IsDestroyed ) {
                return;
            }
            CurHp -= damageAmount;
            if ( IsDestroyed ) {
                SpriteRenderer.color = Color.red;
                OnDestroyed?.Invoke();
            }
        }
    }
}
