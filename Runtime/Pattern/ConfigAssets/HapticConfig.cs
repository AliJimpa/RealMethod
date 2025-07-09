using System;
using UnityEngine;

namespace RealMethod
{
    public abstract class HapticConfig : ConfigAsset
    {
        public abstract void Play(object author);
    }

    public abstract class HapticConfig<T, J> : HapticConfig where T : Enum where J : UnityEngine.Object
    {
        [Header("Type")]
        [SerializeField, Tooltip("The Haptic Config Use Witch Method for Apply Effect")]
        private T Method;

        public sealed override void Play(object author)
        {
            OnProduce(Method, GetInitiator(author));
        }

        protected abstract J GetInitiator(object author);
        protected abstract bool OnProduce(T method, J initiator);
    }





}