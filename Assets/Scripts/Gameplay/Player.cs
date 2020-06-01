using STP.Utils;
using System;
using UnityEngine;

namespace STP.Gameplay {
    public class Player : GameBehaviour{
        int   speed       = 200;
        float maxRotation = 30f;
        
        protected override void CheckDescription() { }

        public void FixedUpdate() {
            var horizontalDirection = Input.GetAxis("Horizontal");
            var verticalDirection   = Input.GetAxis("Vertical");
            var xOffset             = speed  * horizontalDirection;
            var yOffest             = speed  * verticalDirection;
            var offsetVector        = new Vector3(xOffset, yOffest, 0.0f);
            var movingVector        = new Vector2(horizontalDirection, verticalDirection);
            var movingVectorN       = movingVector.normalized;
            if ( movingVectorN == Vector2.zero ) {
                movingVectorN = Vector2.right;
            }
            var dstAngle            = Mathf.Atan2(movingVectorN.x, -movingVectorN.y) / Mathf.PI * 180;
            transform.rotation      = Quaternion.AngleAxis(dstAngle, Vector3.forward);
            GetComponent<Rigidbody2D>().velocity = offsetVector;
            //transform.position     += offsetVector;
        }
    }
}