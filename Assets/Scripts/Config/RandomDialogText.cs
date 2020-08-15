using UnityEngine;

using System.Collections.Generic;

namespace STP.Config {
    public sealed class RandomDialogText : BaseDialogText {
        readonly List<string> _options;

        public override string Text => _options[Random.Range(0, _options.Count)];

        public RandomDialogText(List<string> options) {
            _options = options;
        }
    }
}
