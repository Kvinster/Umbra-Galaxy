using STP.State.Core;
using UnityEngine;

using STP.Utils;
using TMPro;

namespace STP.View {
    public class MotherShipPointer : GameBehaviour{

        public RectTransform Pointer;
        
        public RectTransform MotherShip;
        public RectTransform Canvas;

        PlayerShipState _shipState;
        
        bool  _lastMothershipState;
        float _textOffset;

        Rect VisibleRect => new Rect((Vector2)Canvas.position - Canvas.sizeDelta * Canvas.localScale/2, Canvas.sizeDelta * Canvas.localScale);
        
        
        bool MotherShipInVisibleArea {
            get {
                var motherShipSize  = MotherShip.sizeDelta;
                var motherShipRect  = new Rect((Vector2)MotherShip.position - motherShipSize/2, motherShipSize);
                return VisibleRect.Overlaps(motherShipRect);
            }
    }

        
        protected override void CheckDescription() => ProblemChecker.LogErrorIfNullOrEmpty(this);

        public void Init(PlayerShipState shipState) {
            Pointer.gameObject.SetActive(!MotherShipInVisibleArea);
            TryUpdatePointerPosition();
            _shipState = shipState;
        }
        
        void Update() {
            if ( _lastMothershipState != MotherShipInVisibleArea ) {
                Pointer.gameObject.SetActive(!MotherShipInVisibleArea);
            }
            
            TryUpdatePointerPosition();
            _lastMothershipState = MotherShipInVisibleArea;
        }

        void TryUpdatePointerPosition() {
            if ( MotherShipInVisibleArea ) {
                return;
            }

            var visibleRect   = VisibleRect;
            var visibleCenter = visibleRect.center;
            var P             = _shipState.Position;
            var M             = (Vector2) MotherShip.position;
            var PM            = (M - P);
            var PR            = (VisibleRect.max - P);
            var PRN           = PR.normalized;
            var PMN           = PM.normalized;
            var phiAngle      = convertAngle(Mathf.Atan2(PMN.y, PMN.x) * Mathf.Rad2Deg);
            var deltaAngle    = convertAngle(Mathf.Atan2(PRN.y, PRN.x) * Mathf.Rad2Deg);
            
            var intersectionPoint = Vector2.zero;
                if ( phiAngle <= deltaAngle || phiAngle > 360 - deltaAngle ) {
                    //intersection with a right side
                    var xi = visibleRect.xMax - visibleCenter.x;
                    var yi = xi * PMN.y / PMN.x;
                    intersectionPoint.Set(xi, yi);
                }

                if ( (phiAngle > deltaAngle) && (phiAngle <= 180 - deltaAngle) ) {
                    //intersection with a upper side
                    var yi = visibleRect.yMax - visibleCenter.y;
                    var xi = yi * PMN.x / PMN.y;
                    intersectionPoint.Set(xi, yi);
                }

                if ( (phiAngle > 180 - deltaAngle) && (phiAngle <= 180 + deltaAngle) ) {
                    //intersection with a left side
                    var xi = visibleRect.xMin - visibleCenter.x;
                    var yi = xi * PMN.y / PMN.x;
                    intersectionPoint.Set(xi, yi);
                }

                if ( (phiAngle > 180 + deltaAngle) && (phiAngle < 360 - deltaAngle) ) {
                    //intersection with a bottom side
                    var yi = visibleRect.yMin - visibleCenter.y;
                    var xi = yi * PMN.x / PMN.y;
                    intersectionPoint.Set(xi, yi);
                }
                Pointer.position = visibleRect.center + intersectionPoint;
        }

        float convertAngle(float angle) {
            if ( angle < 0 ) {
                angle = 360 + angle;
            }
            if ( angle >= 360 ) {
                var count = (int) (angle) / 360;
                angle -= count*360;
            }
            return angle;
        }
    }
}