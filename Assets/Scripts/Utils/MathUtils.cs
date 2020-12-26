using UnityEngine;

namespace STP.Utils {
    public static class MathUtils {
        public static float GetSmoothRotationAngleOffset(Vector2 up, Vector2 viewVector, float rotationSpeed) {
            var angleDiff = -Vector2.SignedAngle(viewVector, up);
            return LerpFloat(0, angleDiff, rotationSpeed);
        }
        
        public static float LerpFloat(float a, float b, float coeff) {
            coeff = Mathf.Clamp01(coeff);
            return (b - a) * coeff + a;
        }
    }
}