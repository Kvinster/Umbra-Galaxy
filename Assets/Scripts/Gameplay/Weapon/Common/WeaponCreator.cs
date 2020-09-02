﻿using UnityEngine;

using STP.Behaviour.Core.Objects;
using STP.Gameplay.Weapon.Chargeable;
using STP.Gameplay.Weapon.GunWeapon;
using STP.Gameplay.Weapon.ImpulseWeapon;
using STP.Gameplay.Weapon.LanceWeapon;
using STP.Gameplay.Weapon.LaserWeapon;
using STP.Gameplay.Weapon.MissileWeapon;
using STP.Gameplay.Weapon.ShotgunWeapon;

namespace STP.Gameplay.Weapon.Common {
    public sealed class WeaponCreator {
        public IWeaponControl GetManualWeapon(WeaponType weaponType, BaseShip ownerShip) {
            switch ( weaponType ) {
                case WeaponType.Gun:
                    return new BulletManualControl(new Gun(800, 0.1f));
                case WeaponType.Laser:
                    return new LaserManualControl(new Laser(ownerShip.WeaponMountPoint,
                        ownerShip.GetComponent<Collider2D>()));
                case WeaponType.Lance:
                    return new ChargeableManualControl(new Lance(ownerShip.WeaponMountPoint,
                        ownerShip.GetComponent<Collider2D>()));
                case WeaponType.Impulse:
                    return new ChargeableManualControl(new Impulse());
                case WeaponType.Shotgun:
                    return new BulletManualControl(new Shotgun(800, 0.5f));
                case WeaponType.MissileLauncher:
                    return new BulletManualControl(new MissileLauncher(800, 1f));
            }
            return null;
        }

        public IWeaponControl GetAIWeaponController(WeaponType weaponType, BaseEnemyShip enemyShip) {
            switch ( weaponType ) {
                case WeaponType.Gun:
                    return new BulletAIControl(new Gun(800, 0.5f), enemyShip);
                case WeaponType.Laser:
                    return new LaserAIControl(
                        new Laser(enemyShip.WeaponMountPoint, enemyShip.GetComponent<Collider2D>()), enemyShip);
                case WeaponType.Lance:
                    return new ChargeableAIControl(
                        new Lance(enemyShip.WeaponMountPoint, enemyShip.GetComponent<Collider2D>()), enemyShip);
                case WeaponType.Impulse:
                    return new ChargeableAIControl(new Impulse(), enemyShip);
                case WeaponType.Shotgun:
                    return new BulletAIControl(new Shotgun(800, 0.5f), enemyShip);
                case WeaponType.MissileLauncher:
                    return new BulletAIControl(new MissileLauncher(800, 1f), enemyShip);
            }
            return null;
        }
    }
}