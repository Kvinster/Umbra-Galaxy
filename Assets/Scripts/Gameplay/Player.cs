using STP.Utils;
using System;
using UnityEngine;

namespace STP.Gameplay {
    public class Player : GameBehaviour{
        const int   Speed        = 200;
        const int   BulletSpeed  = 400;
        const float BulletPeriod = 0.1f;
        
        public Transform     BulletLauncher;
        public BulletCreator BulletCreator;

        float _timer;
        
        Rigidbody2D _rigidbody2D;
        
        protected override void CheckDescription() { }

        void Start() {
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }
        
        void Update() {
            TryMovePlayer();
            TryShoot();
        }

        void TryMovePlayer() {
            var horizontalDirection = Input.GetAxis("Horizontal");
            var verticalDirection   = Input.GetAxis("Vertical");
            if ( Mathf.Abs(horizontalDirection) > float.Epsilon || Mathf.Abs(verticalDirection) > float.Epsilon ) {
                var xOffset             = Speed  * horizontalDirection;
                var yOffest             = Speed  * verticalDirection;
                var offsetVector        = new Vector3(xOffset, yOffest, 0.0f);
                var movingVector        = new Vector2(horizontalDirection, verticalDirection);
                var movingVectorN       = movingVector.normalized;
                var dstAngle            = Mathf.Atan2(movingVectorN.x, -movingVectorN.y) / Mathf.PI * 180;
                transform.rotation      = Quaternion.AngleAxis(dstAngle, Vector3.forward);
                _rigidbody2D.velocity = offsetVector;
            }
        }

        void TryShoot() {
            _timer += Time.deltaTime;
            if ( Input.GetButton("Fire1") && (_timer > BulletPeriod) ) {
                var direction = transform.rotation * Vector3.down;
                BulletCreator.CreateBulletOn(BulletLauncher.position, direction, BulletSpeed);
                _timer = 0f;
            }
        }
    }
}