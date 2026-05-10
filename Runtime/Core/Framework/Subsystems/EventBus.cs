using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public sealed class EventBus : GameSubsystem
    {
        private readonly object _lock = new();
        private Dictionary<Type, List<Delegate>> _subscribers => Data.Events;



        // GameSubsystem Methods
        protected override void OnBegin()
        {
        }
        protected override void OnEnd()
        {
        }





        public void Subscribe<T>(Action<T> listener)
        {
            var type = typeof(T);

            if (!_subscribers.TryGetValue(type, out var listeners))
            {
                listeners = new List<Delegate>();
                _subscribers[type] = listeners;
            }

            if (!listeners.Contains(listener))
            {
                listeners.Add(listener);
            }
        }
        public void Unsubscribe<T>(Action<T> listener)
        {
            var type = typeof(T);

            if (!_subscribers.TryGetValue(type, out var listeners))
                return;

            listeners.Remove(listener);

            if (listeners.Count == 0)
            {
                _subscribers.Remove(type);
            }
        }
        public void Publish<T>(T eventData)
        {
            var type = typeof(T);

            if (!_subscribers.TryGetValue(type, out var listeners))
                return;

            var listenersCopy = listeners.ToArray();

            foreach (var listener in listenersCopy)
            {
                try
                {
                    ((Action<T>)listener)?.Invoke(eventData);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }
        public void Clear()
        {
            _subscribers.Clear();
        }


#if UNITY_EDITOR
        protected override string GetInspectorInfor()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < _subscribers.Count; i++)
            {
                var type = _subscribers.GetKey(i);
                List<Delegate> weakRef = _subscribers.GetValue(i);
                sb.AppendLine($"{i}. {type.Name} -> Listener ({weakRef.Count})");
            }

            return sb.ToString();
        }
#endif

    }

}