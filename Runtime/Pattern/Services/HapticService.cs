
using UnityEngine;

namespace RealMethod
{
    public class HapticController
    {
        public void Initiate(Haptic service)
        {

        }
    }

    public abstract class Haptic : Service
    {
        private static Haptic CacheInstance = null;
        private static Haptic instance
        {
            get
            {
                if (CacheInstance != null)
                {
                    return CacheInstance;
                }
                else
                {
                    if (Game.TryFindService(out Haptic CacheInstance))
                    {
                        return CacheInstance;
                    }
                    Debug.LogError("HapticService should add in Game");
                    return CacheInstance;
                }
            }
        }

        public static void Play(HapticConfig config)
        {
            if (instance != null)
                instance.OnPlay(config);
        }
        public static HapticController Produce(HapticConfig confi)
        {
            if (instance == null)
            {
                return null;
            }


            HapticController controller = new HapticController();
            controller.Initiate(instance);
            return controller;
        }

        protected abstract void OnPlay(HapticConfig config);
        protected abstract void OnProduce(HapticController controller);
    }

    // public abstract class Haptic<T> : Haptic where T : HapticConfig
    // {
    //     protected sealed override void OnPlay<U>(U config)
    //     {
    //         if (config is T result)
    //         {
    //             OnPlay(result);
    //         }
    //         else
    //         {
    //             Debug.LogWarning($"Cast config faield target confi {config} should be {typeof(T)}");
    //         }
    //     }

    //     protected abstract void OnPlay(T config);
    // }


    public abstract class HapticConfig : ConfigAsset
    {
        //public abstract void Play(object author);
    }

    // public abstract class HapticConfig<T, J> : HapticConfig where T : Enum where J : UnityEngine.Object
    // {
    //     [Header("Type")]
    //     [SerializeField, Tooltip("The Haptic Config Use Witch Method for Apply Effect")]
    //     private T Method;

    //     public sealed override void Play(object author)
    //     {
    //         OnProduce(Method, GetInitiator(author));
    //     }

    //     protected abstract J GetInitiator(object author);
    //     protected abstract bool OnProduce(T method, J initiator);
    // }



}