using PlayFab;
using UnityEngine;

namespace STP.Core.Leaderboards {
	public abstract class BasePlayfabOperation {
		protected bool IsLoggedIn => PlayFabClientAPI.IsClientLoggedIn();

		protected void LogError(PlayFabError error) {
			Debug.LogError($"Playfab operation failed. Reason: {error.ErrorMessage}");
		}
		
	}
}