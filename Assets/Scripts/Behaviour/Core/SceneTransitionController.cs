using UnityEngine;

using System;

using STP.Behaviour.Starter;
using STP.Utils.GameComponentAttributes;

using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace STP.Behaviour.Core {
	public sealed class SceneTransitionController : BaseCoreComponent {
		static readonly int Progress = Shader.PropertyToID("_Progress");
		static readonly int Center   = Shader.PropertyToID("_Center");

		[NotNull] public SpriteRenderer SpriteRenderer;

		public float FadeInDuration;
		public float FadeOutDuration;

		bool _isAnimPlaying;

		Camera    _camera;
		Transform _playerTransform;

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

		void Update() {
			if ( !IsInit ) {
				return;
			}
			if ( !_camera ) {
				return;
			}
			var cameraPos = _camera.transform.position;
			transform.position = new Vector3(cameraPos.x, cameraPos.y, 0);
		}

		protected override void InitInternal(CoreStarter starter) {
			_camera          = starter.MainCamera;
			_playerTransform = starter.Player.transform;

			UniTask.Create(PlayShowAnim);
		}

		public async UniTask PlayHideAnim(Vector3 fadeInWorldCenter) {
			if ( _isAnimPlaying ) {
				var error = "Anim is already playing";
				Debug.LogError(error);
				throw new OperationCanceledException(error);
			}

			_isAnimPlaying = true;
			await CreateHideAnim(fadeInWorldCenter);
			_isAnimPlaying = false;
		}

		async UniTask PlayShowAnim() {
			if ( _isAnimPlaying ) {
				var error = "Anim is already playing";
				Debug.LogError(error);
				return;
			}

			_isAnimPlaying = true;
			await CreateShowAnim(_playerTransform.position);
			_isAnimPlaying = false;
		}

		Sequence CreateHideAnim(Vector3 fadeInWorldCenter) {
			var fadeInScreenCenter = _camera.WorldToScreenPoint(fadeInWorldCenter) -
			                         new Vector3(Screen.width / 2f, Screen.height / 2f);
			var progress = 0f;
			MaterialPropertyBlock.SetVector(Center, fadeInScreenCenter);
			MaterialPropertyBlock.SetFloat(Progress, progress);
			SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
			var anim = DOTween.Sequence()
				.Append(DOTween.To(() => progress, x => {
					progress = x;
					MaterialPropertyBlock.SetFloat(Progress, progress);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				}, 1f, FadeInDuration));
			anim.SetUpdate(UpdateType.Normal, true);
			return anim;
		}

		Sequence CreateShowAnim(Vector3 fadeOutWorldCenter) {
			var fadeOutScreenCenter = _camera.transform.TransformPoint(fadeOutWorldCenter);
			var progress            = 1f;
			MaterialPropertyBlock.SetFloat(Progress, progress);
			MaterialPropertyBlock.SetVector(Center, fadeOutScreenCenter);
			SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
			var anim = DOTween.Sequence()
				.Append(DOTween.To(() => progress, x => {
					progress = x;
					MaterialPropertyBlock.SetFloat(Progress, progress);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				}, 0f, FadeOutDuration));
			anim.SetUpdate(UpdateType.Normal, true);
			return anim;
		}
	}
}
