using UnityEngine;

using System.Collections.Generic;

namespace STP.Behaviour.Utils {
    public sealed class RandomSprite : MonoBehaviour {
        public SpriteRenderer SpriteRenderer;
        [Space]
        public List<Sprite> Sprites = new List<Sprite>();

        void Reset() {
            SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void Start() {
            SpriteRenderer.sprite = ((Sprites == null) || (Sprites.Count == 0))
                ? null
                : Sprites[Random.Range(0, Sprites.Count)];
        }
    }
}
