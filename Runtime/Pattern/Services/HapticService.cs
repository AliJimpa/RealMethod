
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{

    public interface IHapticProvider
    {
        void Play();
        void Pause();
        void Stop();
        HapticConfig GetConfig();
    }

    public abstract class Haptic : Service
    {
        private class HapticController : IHapticProvider
        {
            public HapticConfig hapticConfig { get; private set; }

            public HapticController(HapticConfig config)
            {
                hapticConfig = config;
            }
            void IHapticProvider.Play()
            {
                instance.OnPlay(hapticConfig);
            }
            void IHapticProvider.Pause()
            {
                instance.OnPause(hapticConfig);
            }
            void IHapticProvider.Stop()
            {
                instance.OnStop(hapticConfig);
            }
            HapticConfig IHapticProvider.GetConfig()
            {
                return hapticConfig;
            }
        }

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
                    Debug.LogError("Your HapticService should added [Warning:Use Just One HapticService]");
                    return CacheInstance;
                }
            }
        }
        private List<HapticController> List = new List<HapticController>(5);


        // Public Methods
        public static IHapticProvider Produce(HapticConfig config)
        {
            if (instance == null)
            {
                return null;
            }

            HapticController controller = new HapticController(config);
            instance.List.Add(controller);
            return controller;
        }
        public static bool Demolish(IHapticProvider target)
        {
            if (instance == null)
            {
                return false;
            }
            foreach (var item in instance.List)
            {
                if (item.hapticConfig == target.GetConfig())
                {
                    return instance.List.Remove(item);
                }
            }
            return false;
        }
        public static void Play(HapticConfig config)
        {
            if (instance == null)
            {
                return;
            }

            instance.OnPlay(config);
        }
        public static bool TryFindProvider(string hapticName , out IHapticProvider provider)
        {
            if (instance == null)
            {
                provider = null;
                return false;
            }

            foreach (var item in instance.List)
            {
                if (item.hapticConfig.name == hapticName)
                {
                    provider = item;
                    return true;
                }
            }

            provider = null;
            return false;
        }


        // Abstract Methods
        protected abstract void OnPlay(HapticConfig config);
        protected abstract void OnPause(HapticConfig config);
        protected abstract void OnStop(HapticConfig config);
    }

    public abstract class HapticConfig : ConfigAsset
    {

    }

}