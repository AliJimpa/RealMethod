using System;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "EventAsset", menuName = "RealMethod/Delegate/Event", order = 1)]
    public class EventAsset : DelegateAsset
    {
        private event Action _event;

        public override void Invoke()
        {
            _event?.Invoke();
        }
        public override void AddListener<T>(T listener)
        {
            if (listener is Action provider)
            {
                _event += provider;
            }
            else
            {
                Debug.LogWarning($"The listener should be {typeof(Action)} for binding, AddListener Faild!");
            }
        }
        public override void RemoveListener<T>(T listener)
        {
            if (listener is Action provider)
            {
                _event -= provider;
            }
            else
            {
                Debug.LogWarning($"The listener should be {typeof(Action)} for binding, AddListener Faild!");
            }
        }



        // -------- Operator Overloads --------
        public static EventAsset operator +(EventAsset asset, Action listener)
        {
            asset._event += listener;
            return asset;
        }
        public static EventAsset operator -(EventAsset asset, Action listener)
        {
            asset._event -= listener;
            return asset;
        }
    }
}