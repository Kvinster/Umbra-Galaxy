using System;
using System.Collections.Generic;

namespace STP.Utils.Events {
    public sealed class Handler<T> : IHandler where T : struct {
        class ActionWrapper {
            public bool               NeedToDelete;
            public readonly Action<T> Action;
	    
            public ActionWrapper(Action<T> action) {
                Action       = action;
                NeedToDelete = false;
            }
        }
		
        readonly List<ActionWrapper> _actions = new List<ActionWrapper>();
        readonly List<ActionWrapper> _toAdd   = new List<ActionWrapper>(1);

        int _fireDepth = 0;
        
        public void Add(Action<T> action) {
            if ( action == null ) {
                return;
            }
            if ( _fireDepth == 0 ) {
                _actions.Add(new ActionWrapper(action));
            } else {
                _toAdd.Add(new ActionWrapper(action));
            }
        }

        public void Remove(Action<T> action) {
            var wrappers = _actions.FindAll((x) => x.Action == action);
            foreach ( var wrapper in wrappers ) {
                wrapper.NeedToDelete = true;
            }
        }

        public void Fire(T ev) {
            ++_fireDepth;
            foreach ( var wrapper in _actions ) {
                if ( !wrapper.NeedToDelete ) {
                    wrapper.Action.Invoke(ev);
                }
            }
            _fireDepth = Math.Max(0, _fireDepth - 1);
            if ( _fireDepth == 0 ) {
                _actions.RemoveAll((x) => x.NeedToDelete);
                if ( _toAdd.Count > 0 ) {
                    _actions.AddRange(_toAdd);
                    _toAdd.Clear();
                }
            }
        }
    }
}