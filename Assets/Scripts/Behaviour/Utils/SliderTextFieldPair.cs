using UnityEngine;
using UnityEngine.UI;

using System;

using TMPro;

namespace STP.Behaviour.Utils {
    public sealed class SliderTextFieldPair : MonoBehaviour {
        public Slider         Slider;
        public TMP_InputField InputField;
        public TMP_Text       MinValueText;
        public TMP_Text       MaxValueText;

        bool _isInit;

        int _minValue;
        int _maxValue;

        int _curValue = int.MinValue;
        public int CurValue {
            get => _curValue;
            private set {
                if ( _curValue == value ) {
                    return;
                }
                _curValue = value;
                OnValueChanged?.Invoke(_curValue);
            }
        }

        public event Action<int> OnValueChanged;

        public void CommonInit() {
            Slider.onValueChanged.AddListener(OnSliderValueChanged);
            InputField.onValueChanged.AddListener(OnTextValueChanged);
            InputField.characterValidation = TMP_InputField.CharacterValidation.Digit;
            InputField.onEndEdit.AddListener(OnTextEndEdit);
        }

        public void Init(int minValue, int maxValue) {
            SetBorderValues(minValue, maxValue);
        }

        public void SetBorderValues(int minValue, int maxValues) {
            if ( minValue > maxValues ) {
                Debug.LogErrorFormat("Invalid border values: '{0}', '{1}'", minValue, maxValues);
                return;
            }
            _minValue = minValue;
            _maxValue = maxValues;

            if ( MinValueText ) {
                MinValueText.text = _minValue.ToString();
            }
            if ( MaxValueText ) {
                MaxValueText.text = _maxValue.ToString();
            }
            
            Slider.minValue = _minValue;
            Slider.maxValue = _maxValue;
            
            CurValue = _minValue;
            Slider.value = CurValue;
        }

        public void Deinit() {
            _curValue = int.MinValue;
        }

        void OnSliderValueChanged(float newValueRaw) {
            var newValue = Mathf.RoundToInt(newValueRaw);
            CurValue = newValue;
            InputField.text = CurValue.ToString();
        }

        void OnTextValueChanged(string newValueText) {
            if ( CheckValueText(newValueText, out var newValue) ) {
                CurValue = newValue;
                Slider.value = CurValue;
            }
        }

        void OnTextEndEdit(string text) {
            if ( CheckValueText(text, out var value) ) {
                CurValue = value;
                Slider.value = CurValue;
            } else {
                CurValue = Mathf.RoundToInt(Slider.value);
                InputField.text = CurValue.ToString();
            }
        }

        bool CheckValueText(string valueText, out int value) {
            if ( int.TryParse(valueText, out var newValue) && (newValue >= _minValue) && (newValue <= _maxValue) ) {
                value = newValue;
                return true;
            }
            value = -1;
            return false;
        }
    }
}
