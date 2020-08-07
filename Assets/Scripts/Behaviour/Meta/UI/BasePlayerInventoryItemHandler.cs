using UnityEngine;

using STP.Utils;

namespace STP.Behaviour.Meta.UI {
    public abstract class BasePlayerInventoryItemHandler : GameBehaviour {
        public PlayerInventoryView CurPlayerInventoryView { get; protected set; } 

        public void RegisterPlayerInventoryView(PlayerInventoryView playerInventoryView) {
            if ( CurPlayerInventoryView != null ) {
                Debug.LogError("Can't register PlayerInventoryView - another PlayerInventoryView already registered");
                return;
            }
            CurPlayerInventoryView = playerInventoryView;
            OnPlayerInventoryViewRegistered();
        }

        public void UnregisterPlayerInventoryView(PlayerInventoryView playerInventoryView) {
            if ( !CurPlayerInventoryView ) {
                Debug.LogError("Can't unregister PlayerInventoryView - no PlayerInventoryView registered");
                return;
            }
            if ( CurPlayerInventoryView != playerInventoryView ) {
                Debug.LogError("Can't unregister PlayerInventoryView - another PlayerInventoryView registered");
                return;
            }
            CurPlayerInventoryView = null;
            OnPlayerInventoryViewUnregistered(playerInventoryView);
        }

        protected virtual void OnPlayerInventoryViewRegistered() { }
        protected virtual void OnPlayerInventoryViewUnregistered(PlayerInventoryView playerInventoryView) { }
    }
}
