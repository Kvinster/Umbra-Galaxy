﻿using UnityEngine;

using STP.View;

namespace STP.Gameplay {
    public class MotherShip : CoreBehaviour{
        public Transform  TeleportPoint;
        CoreOverlayHelper _overlayHelper;

        protected override void CheckDescription() { }

        public override void Init(CoreStarter starter) {
            _overlayHelper = starter.OverlayHelper;
            starter.CoreManager.MotherShipState.TeleportPosition = TeleportPoint.position;
        }
        
        void OnTriggerEnter2D(Collider2D other) {
            var playerComp = other.gameObject.GetComponent<Player>();
            if ( playerComp ) {
                _overlayHelper.ShowMothershipOverlay();
            }
        }
        
        void OnTriggerExit2D(Collider2D other) {
            _overlayHelper.HideMothershipOverlay();
        }
    }
}