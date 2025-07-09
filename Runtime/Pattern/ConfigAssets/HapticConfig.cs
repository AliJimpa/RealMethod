using System;
using UnityEngine;

namespace RealMethod
{
    public abstract class HapticConfig : ConfigAsset
    {
        public abstract void Play();
    }

    public abstract class HapticConfig<T, J> : HapticConfig where T : Enum where J : struct
    {
        [Header("Type")]
        [SerializeField, Tooltip("The Haptic Config Use Witch Method for Apply Effect")]
        private T Method;

        public override void Play()
        {
            OnProduce(Method);
        }
        protected abstract J OnProduce(T method);
    }



}