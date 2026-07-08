using System;
using UnityEngine;

namespace RealMethod
{
    public abstract class DelegateAsset : UniqueAsset
    {
        public abstract void Invoke();

        public abstract void AddListener<D>(D listener) where D : Delegate;
        public abstract void RemoveListener<D>(D listener) where D : Delegate;

#if UNITY_EDITOR
        [ContextMenu("Invoke")]
        private void Raise()
        {
            Invoke();
        }
#endif
    }

    public abstract class EventAsset<T> : DelegateAsset
    {
        [SerializeField]
        private bool CanUseDefaultValue = false;
        [SerializeField, ConditionalHide("CanUseDefaultValue", true, false)]
        private T DefaultValue;
        private event Action<T> _event;


        public void Invoke(T value)
        {
            _event?.Invoke(value);
        }

        public override void Invoke()
        {
            if (CanUseDefaultValue)
            {
                _event?.Invoke(DefaultValue);
            }
            else
            {
                Debug.LogWarning($"You can Invoke {GetType().Name} Event with Defaut value, [CanUseDefaultValue = false]");
            }
        }
        public override void AddListener<D>(D listener)
        {
            if (listener is Action<T> provider)
            {
                _event += provider;
            }
            else
            {
                Debug.LogWarning($"The listener should be {typeof(Action<T>)} for binding, AddListener Faild!");
            }
        }
        public override void RemoveListener<D>(D listener)
        {
            if (listener is Action<T> provider)
            {
                _event -= provider;
            }
            else
            {
                Debug.LogWarning($"The listener should be {typeof(Action<T>)} for binding, AddListener Faild!");
            }
        }


        // -------- Operator Overloads --------
        public static EventAsset<T> operator +(EventAsset<T> asset, Action<T> listener)
        {
            asset._event += listener;
            return asset;
        }
        public static EventAsset<T> operator -(EventAsset<T> asset, Action<T> listener)
        {
            asset._event -= listener;
            return asset;
        }
    }
    public abstract class EventAsset<T, J> : DelegateAsset
    {
        [SerializeField]
        private bool CanUseDefaultValue = false;
        [SerializeField, ConditionalHide("CanUseDefaultValue", true, false)]
        private T DefaultValue_st;
        [SerializeField, ConditionalHide("CanUseDefaultValue", true, false)]
        private J DefaultValue_nd;

        private event Action<T, J> _event;

        public void Invoke(T value, J value2)
        {
            _event?.Invoke(value, value2);
        }


        public override void Invoke()
        {
            if (CanUseDefaultValue)
            {
                _event?.Invoke(DefaultValue_st, DefaultValue_nd);
            }
            else
            {
                Debug.LogWarning($"You can Invoke {GetType().Name} Event with Defaut value");
            }
        }
        public override void AddListener<D>(D listener)
        {
            if (listener is Action<T, J> provider)
            {
                _event += provider;
            }
            else
            {
                Debug.LogWarning($"The listener should be {typeof(Action<T, J>)} for binding, AddListener Faild!");
            }
        }
        public override void RemoveListener<D>(D listener)
        {
            if (listener is Action<T, J> provider)
            {
                _event -= provider;
            }
            else
            {
                Debug.LogWarning($"The listener should be {typeof(Action<T, J>)} for binding, AddListener Faild!");
            }
        }

        // -------- Operator Overloads --------
        public static EventAsset<T, J> operator +(EventAsset<T, J> asset, Action<T, J> listener)
        {
            asset._event += listener;
            return asset;
        }
        public static EventAsset<T, J> operator -(EventAsset<T, J> asset, Action<T, J> listener)
        {
            asset._event -= listener;
            return asset;
        }
    }
    public abstract class EventAsset<T, J, F> : DelegateAsset
    {
        [SerializeField]
        private bool CanUseDefaultValue = false;
        [SerializeField, ConditionalHide("CanUseDefaultValue", true, false)]
        private T DefaultValue_st;
        [SerializeField, ConditionalHide("CanUseDefaultValue", true, false)]
        private J DefaultValue_nd;
        [SerializeField, ConditionalHide("CanUseDefaultValue", true, false)]
        private F DefaultValue_rd;

        private event Action<T, J, F> _event;

        public void Invoke(T value, J value2, F value3)
        {
            _event?.Invoke(value, value2, value3);
        }

        public override void Invoke()
        {
            if (CanUseDefaultValue)
            {
                _event?.Invoke(DefaultValue_st, DefaultValue_nd, DefaultValue_rd);
            }
            else
            {
                Debug.LogWarning($"You can Invoke {GetType().Name} Event with Defaut value");
            }
        }
        public override void AddListener<D>(D listener)
        {
            if (listener is Action<T, J, F> provider)
            {
                _event += provider;
            }
            else
            {
                Debug.LogWarning($"The listener should be {typeof(Action<T, J, F>)} for binding, AddListener Faild!");
            }
        }
        public override void RemoveListener<D>(D listener)
        {
            if (listener is Action<T, J, F> provider)
            {
                _event -= provider;
            }
            else
            {
                Debug.LogWarning($"The listener should be {typeof(Action<T, J, F>)} for binding, AddListener Faild!");
            }
        }

        // -------- Operator Overloads --------
        public static EventAsset<T, J, F> operator +(EventAsset<T, J, F> asset, Action<T, J, F> listener)
        {
            asset._event += listener;
            return asset;
        }
        public static EventAsset<T, J, F> operator -(EventAsset<T, J, F> asset, Action<T, J, F> listener)
        {
            asset._event -= listener;
            return asset;
        }
    }
}