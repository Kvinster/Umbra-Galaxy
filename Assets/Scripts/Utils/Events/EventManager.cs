using UnityEngine;

using System;
using System.Collections.Generic;

namespace STP.Utils.Events {
    public static class EventManager {
        static readonly Dictionary<Type, IHandler> Handlers = new Dictionary<Type, IHandler>();
        
        public static void Subscribe<T>(Action<T> action) where T : struct {
            GetOrCreateHandler<T>()?.Add(action);
        }

        public static void Unsubscribe<T>(Action<T> action) where T : struct {
            GetOrCreateHandler<T>()?.Remove(action);
        }

        public static void Fire<T>(T ev) where T : struct {
            GetOrCreateHandler<T>()?.Fire(ev);
        }

        static Handler<T> GetOrCreateHandler<T>() where T : struct {
            var eventType = typeof(T);
            if ( !Handlers.TryGetValue(eventType, out var genericHandler) ) {
                genericHandler = new Handler<T>();
                Handlers.Add(eventType, genericHandler);
            }
            if ( !(genericHandler is Handler<T> handler) ) {
                Debug.LogError("Invalid handler");
                return null;
            }
            return handler;
        }
    }
}