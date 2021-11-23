using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.VFX;

using STP.Behaviour.Utils;
using STP.Manager;
using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;

namespace STP.Behaviour.Core {
	public sealed class Portal : GameComponent {
		const string PortalSizeId         = "PortalSize";
		const float  LevelStartPortalSize = 1f;
		const float  LevelWinPortalSize   = 1.5f;

		const string AppearEventName              = "OnAppear";
		const string AppearTimeId                 = "AppearBorder_AppearTime";
		const string AppearMainLifetimeId         = "AppearBorder_MainLifetime";
		const string AppearDisappearTimeId        = "AppearBorder_DisappearTime";
		const string AppearParticlesKillboxSizeId = "AppearBorder_ParticlesKillboxSize";
		const float  PlayerScaleTime              = 1f;

		[Header("Parameters")]
		public float PlayerAnimTime;
		public float DisappearTime;
		public float DistanceToPlayer;
		public float ExplosionStartTime;
		[Header("Dependencies")]
		[NotNull] public VisualEffect VisualEffect;
		[NotNull] public LevelExplosionZone LevelWinExplosionZone;

		Player             _player;
		Transform          _playerStartPos;
		LevelGoalManager   _levelGoalManager;
		LevelManager       _levelManager;
		CoreWindowsManager _windowsManager;

		Camera _camera;

		bool _disappearStarted;

		Tween _levelStartAnim;
		Tween _levelWinStartAnim;
		Tween _levelWinFinishAnim;

		bool IsInit => (_levelManager != null);

		void OnDisable() {
			_levelStartAnim?.Kill();
			_levelWinStartAnim?.Kill();
			_levelWinFinishAnim?.Kill();
		}

		public void Init(Player player, Transform playerStartPos, LevelGoalManager levelGoalManager,
			LevelManager levelManager, CoreWindowsManager windowsManager) {
			Assert.IsFalse(IsInit);
			_player           = player;
			_playerStartPos   = playerStartPos;
			_levelGoalManager = levelGoalManager;
			_levelManager     = levelManager;
			_windowsManager   = windowsManager;

			_levelManager.OnLevelWinStarted += StartLevelWinAnim;

			_camera = CameraUtility.Instance.Camera;

			gameObject.SetActive(false);
			VisualEffect.Stop();
			LevelWinExplosionZone.SetActive(false);

			StartLevelStartAnim(true);
		}

		void StartLevelStartAnim(bool firstSpawn) {
			Assert.IsTrue(IsInit);
			gameObject.SetActive(true);
			var playerTransform = _player.transform;
			var pos             = _playerStartPos.position;
			var appearTime      = VisualEffect.GetFloat(AppearTimeId);
			var mainLifetime    = VisualEffect.GetFloat(AppearMainLifetimeId);
			VisualEffect.SetFloat(AppearParticlesKillboxSizeId, 0f);
			VisualEffect.SetFloat(PortalSizeId, LevelStartPortalSize);
			transform.position         = pos;
			playerTransform.position   = pos;
			playerTransform.localScale = Vector3.zero;
			_windowsManager.HideLevelUi();
			_levelStartAnim = DOTween.Sequence()
				.AppendCallback(() => VisualEffect.SendEvent(AppearEventName))
				.Join(DOTween.To(() => VisualEffect.GetFloat(AppearParticlesKillboxSizeId),
					x => VisualEffect.SetFloat(AppearParticlesKillboxSizeId, x), 1f, appearTime).SetEase(Ease.Linear))
				.Insert(appearTime, playerTransform.DOScale(Vector3.one, PlayerScaleTime))
				.Insert(appearTime + mainLifetime,
					DOTween.To(() => VisualEffect.GetFloat(AppearParticlesKillboxSizeId),
						x => VisualEffect.SetFloat(AppearParticlesKillboxSizeId, x), 0f,
						VisualEffect.GetFloat(AppearDisappearTimeId)).SetEase(Ease.Linear))
				.OnComplete(() => {
					if ( firstSpawn ) {
						_levelManager.ActivateLevel();
					}
					_levelStartAnim = null;
					VisualEffect.Stop();
					gameObject.SetActive(false);
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
			}
		}
	}
}
