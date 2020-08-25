using UnityEngine;

using System.Collections.Generic;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Utils {
    public sealed class RandomSprite : GameComponent {
        [NotNull]
        public SpriteRenderer SpriteRenderer;
        [Space] [NotNullOrEmpty]
        public List<Sprite> Sprites = new List<Sprite>();

        void Reset() {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void Start() {
            SpriteRenderer.sprite = Sprites[Random.Range(0, Sprites.Count)];
        }
    }
}
