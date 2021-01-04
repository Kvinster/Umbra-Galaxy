using UnityEngine;
using UnityEngine.SceneManagement;

using System;

using STP.Utils;
using STP.Utils.GameComponentAttributes;

using DG.Tweening;
using RSG;

namespace STP.Behaviour.Core {
	public sealed class SceneTransitionController : GameComponent {
		static readonly int Progress = Shader.PropertyToID("_Progress");
		static readonly int Center   = Shader.PropertyToID("_Center");

		public static SceneTransitionController Instance { get; private set; }

		[NotNull] public SpriteRenderer SpriteRenderer;

		public float FadeInDuration;
		public float FadeOutDuration;

		Sequence _anim;

		Camera _camera;

		MaterialPropertyBlock _mpb;
		MaterialPropertyBlock MaterialPropertyBlock {
			get {
				if ( _mpb == null ) {
					_mpb = new MaterialPropertyBlock();
					SpriteRenderer.GetPropertyBlock(_mpb);
				}
				return _mpb;
			}
		}

		Camera CachedCamera {
			get {
				if ( !_camera ) {
					_camera = Camera.main;
				}
				return _camera;
			}
		}

		protected override void Awake() {
			base.Awake();
			if ( !Instance || (Instance == this) ) {
				Instance = this;
				DontDestroyOnLoad(gameObject);
			} else {
				Destroy(gameObject);
			}
		}

		void OnDestroy() {
			if ( Instance == this ) {
				Instance = null;
			}
		}

		void Update() {
			var cameraPos = CachedCamera.transform.position;
			transform.position = new Vector3(cameraPos.x, cameraPos.y, 0);
		}

		public IPromise Transition(string sceneName, Vector3 fadeInWorldCenter) {
			return Transition(sceneName, fadeInWorldCenter, () => CachedCamera.transform.TransformPoint(Vector3.zero));
		}

		public IPromise Transition(string sceneName, Vector3 fadeInWorldCenter, Func<Vector3> fadeOutWorldCenterGetter) {
			if ( _anim != null ) {
				var error = "Scene transition already playing";
				Debug.LogError(error);
				return Promise.Rejected(new Exception(error));
			}
			var fadeInScreenCenter = CachedCamera.WorldToScreenPoint(fadeInWorldCenter) -
			                         new Vector3(Screen.width / 2f, Screen.height / 2f);
			MaterialPropertyBlock.SetVector(Center, fadeInScreenCenter);
			SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
			var progress = 0f;
			var promise  = new Promise();
			_anim = DOTween.Sequence()
				.Append(DOTween.To(() => progress, x => {
					progress = x;
					MaterialPropertyBlock.SetFloat(Progress, progress);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				}, 1f, FadeInDuration))
				.AppendCallback(() => {
					SceneManager.LoadScene(sceneName);
				})
				.AppendInterval(0.1f)
				.AppendCallback(() => {
					var fadeOutScreenCenter = CachedCamera.WorldToScreenPoint(fadeOutWorldCenterGetter()) -
					                          new Vector3(Screen.width / 2f, Screen.height / 2f);
					MaterialPropertyBlock.SetVector(Center, fadeOutScreenCenter);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				})
				.Append(DOTween.To(() => progress, x => {
					progress = x;
					MaterialPropertyBlock.SetFloat(Progress, progress);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				}, 0f, FadeOutDuration))
				.AppendCallback(() => _anim = null);
			_anim.onComplete += promise.Resolve;
			_anim.SetUpdate(true);
			return promise;
		}
	}
}
