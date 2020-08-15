using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Core.Objects.DoorObject {
    public sealed class DoorFrame : GameBehaviour {
        Vector2 _openedPosition;
        Vector2 _closedPosition;

        Vector2 _from;
        Vector2 _to;
        
        public void Init() {
            var rectTransform = transform as RectTransform;
            _closedPosition = rectTransform.localPosition;
            _openedPosition = _closedPosition - (Vector2)(rectTransform.localRotation * new Vector2(0f, rectTransform.sizeDelta.y));
            SetDoorState(DoorState.Closed);
            SetProgress(1);
        }

        public void SetDoorState(DoorState state) {
            switch ( state ) {
                case DoorState.Closed:
                    _to   = _closedPosition;
                    _from = _closedPosition;
                    break;
                case DoorState.Opened:
                    _to   = _openedPosition;
                    _from = _openedPosition;
                    break;
                case DoorState.Opening:
                    _from = _closedPosition;
                    _to   = _openedPosition;
                    break;
                case DoorState.Closing:
                    _from = _openedPosition;
                    _to   = _closedPosition;
                    break;
                default:
                    Debug.LogError("Unknown door state");
                    break;
            }
        }

        public void SetProgress(float progress) {
            transform.localPosition = Vector2.Lerp(_from, _to, progress);
        }
    }
}