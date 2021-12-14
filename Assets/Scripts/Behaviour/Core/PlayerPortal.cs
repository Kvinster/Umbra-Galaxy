﻿using UnityEngine;
using UnityEngine.Assertions;

using STP.Behaviour.Utils;
using STP.Manager;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

using STP.Behaviour.Starter;

namespace STP.Behaviour.Core {
	public sealed class PlayerPortal : Portal {
		[Header("Impl Parameters")]
		public float PlayerAnimTime;
		public float DisappearTime;
		public float DistanceToPlayer;
		public float ExplosionStartTime;
		[Space]
		public float LevelWinPortalSize = 1.5f;
		[Header("Impl Dependencies")]
		[NotNull] public LevelExplosionZone LevelWinExplosionZone;
		[Space]
		[NotNull] public BaseSimpleSoundPlayer PlayerEnterSoundPlayer;
		[NotNull] public BaseSimpleSoundPlayer PlayerAppearSoundPlayer;

		Player             _player;
		Transform          _playerStartPos;
		LevelGoalManager   _levelGoalManager;
		LevelManager       _levelManager;
		CoreWindowsManager _windowsManager;

		Camera _camera;

		bool _disappearStarted;

		Tween _levelWinStartAnim;
		Tween _levelWinFinishAnim;

		bool IsInit => (_levelManager != null);

		protected override void OnDisable() {
			base.OnDisable();
			_levelWinStartAnim?.Kill();
			_levelWinFinishAnim?.Kill();
		}

		public override void Init(CoreStarter coreStarter) {
			base.Init(coreStarter);
			_player           = coreStarter.Player;
			_playerStartPos   = coreStarter.PlayerStartPos;
			_levelGoalManager = coreStarter.LevelGoalManager;
			_levelManager     = coreStarter.LevelManager;
			_windowsManager   = coreStarter.WindowsManager;

			_levelManager.OnLevelWinStarted += StartLevelWinAnim;

			_camera = CameraUtility.Instance.Camera;

			LevelWinExplosionZone.SetActive(false);

			PlayLevelStartAnim();
		}

		void PlayLevelStartAnim() {
			_windowsManager.HideLevelUi();
			PlayerAppearSoundPlayer.Play();
			PlayTargetAppearAnim(_player.transform, _playerStartPos, () => {
				_levelManager.ActivateLevel();
				_windowsManager.ShowLevelUi();
			});
		}

		void StartLevelWinAnim() {
			Assert.IsTrue(IsInit);
			gameObject.SetActive(true);
			transform.position = GetLevelWinPosition();
			VisualEffect.SetFloat(PortalSizeId, LevelWinPortalSize);
			VisualEffect.Play();
			_levelWinStartAnim = DOTween.Sequence()
				.AppendInterval(ExplosionStartTime)
				.AppendCallback(() => LevelWinExplosionZone.SetActive(true));
		}

		Vector2 GetLevelWinPosition() {
			var     shifts     = new [] { Vector2.right, Vector2.left, Vector2.up, Vector2.down };
			var     cameraRect = GetCameraRect();
			Vector2 playerPos  = _player.transform.position;
			foreach ( var shift in shifts ) {
				var pos    = playerPos + shift * DistanceToPlayer;
				var farPos = playerPos + shift * (DistanceToPlayer + 280f);
				if ( cameraRect.Contains(farPos - (Vector2)_camera.transform.position) ) {
					return pos;
				}
			}
			return playerPos;
		}

		Rect GetCameraRect() {
			var height     = _camera.orthographicSize * 2;
			var weight     = height * _camera.aspect;
			var screenSize = new Vector2(weight, height);
			return new Rect(-screenSize / 2f, screenSize);
		}

		void FinishLevelWinAnim(Transform playerTransform) {
			Assert.IsTrue(IsInit);
			Assert.IsFalse(_disappearStarted);
			_disappearStarted = true;
			_levelWinFinishAnim = DOTween.Sequence()
				.Append(playerTransform.DOMove(VisualEffect.transform.position, PlayerAnimTime))
				.Join(playerTransform.DOScale(Vector3.zero, PlayerAnimTime))
				.Join(DOTween.To(() => VisualEffect.GetFloat(PortalSizeId), x => VisualEffect.SetFloat(PortalSizeId, x),
					0f, DisappearTime))
				.OnComplete(_levelManager.FinishLevelWin);
		}

		void OnTriggerEnter2D(Collider2D other) {
			if ( !IsInit || !_levelGoalManager.IsLevelWon ) {
				return;
			}
			if ( !_disappearStarted && other.TryGetComponent<Player>(out var player) ) {
				player.DisableMovement();
				FinishLevelWinAnim(player.transform);
				PlayerEnterSoundPlayer.Play();
			}
		}
	}
}
