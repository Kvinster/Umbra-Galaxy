using UnityEngine;

using STP.Behaviour.Starter;
using STP.Behaviour.Utils;
using STP.Utils.GameComponentAttributes;

namespace STP.Behaviour.Core {
	[RequireComponent(typeof(RectTransform))]
	public sealed class BossChunk : BaseCoreComponent {
		[NotNull] public Transform PlayerSpawnPosition;

		[NotNull] public RectTransform OwnTransform;
		
		protected override void InitInternal(CoreStarter starter) {
			// Teleport player to the start position
			OwnTransform.sizeDelta = starter.CameraArea.size;
			starter.Player.transform.position = new Vector3(PlayerSpawnPosition.position.x, PlayerSpawnPosition.position.y, starter.PlayerStartPos.transform.position.z); 
		}
	}
}
