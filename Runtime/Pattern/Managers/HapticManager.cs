using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public interface IHapticProvider : IIdentifier
    {
        void Play();
        void Pause();
        void Stop();
        HapticConfig GetConfig();
    }
    public abstract class HapticManager : MonoBehaviour, IGameManager
    {
        [System.Serializable]
        private class HapticController : IHapticProvider
        {
            [SerializeField]
            private string Label;
            [SerializeField, ReadOnly]
            private HapticConfig Config;
            private HapticManager Owner;

            // Implement IIdentifier Interface
            public string NameID => Config.NameID;
            // Implement IHapticProvider Interface
            public HapticController(HapticManager manager, HapticConfig config)
            {
                Owner = manager;
                if (config == null)
                {
                    Debug.LogWarning("HapticConfig is not Valid!");
                    return;
                }
                Config = config;
                Label = Config.NameID;
            }
            void IHapticProvider.Play()
            {
                Owner.OnPlay(Config);
            }
            void IHapticProvider.Pause()
            {
                Owner.OnPause(Config);
            }
            void IHapticProvider.Stop()
            {
                Owner.OnStop(Config);
            }
            HapticConfig IHapticProvider.GetConfig()
            {
                return Config;
            }
        }

        [Header("Setting")]
        [SerializeField]
        private HapticConfig[] DefaultConfig;
        [SerializeField]
        private List<HapticController> produceList = new List<HapticController>(5);


        // Implement IGameManager Interface
        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            if (!AlwaysLoaded)
            {
                Debug.LogWarning("HapticManager Should initiate in Game Scope");
                return;
            }

            if (Game.TryFindService(out Spawn SpawnServ))
            {
                SpawnServ.BringManager(this);
            }

            if (DefaultConfig != null)
            {
                foreach (var conf in DefaultConfig)
                {
                    Produce(conf);
                }
            }
        }
        public void InitiateService(Service service)
        {
            if (service is Spawn spawnservice)
            {
                spawnservice.BringManager(this);
            }
        }


        // Public Methods
        public IHapticProvider Produce(HapticConfig config)
        {
            HapticController controller = new HapticController(this, config);
            produceList.Add(controller);
            return controller;
        }
        public void Play(HapticConfig config)
        {
            OnPlay(config);
        }
        public bool Demolish(IHapticProvider target)
        {
            foreach (var item in produceList)
            {
                if (item.NameID == target.NameID)
                {
                    return produceList.Remove(item);
                }
            }
            return false;
        }
        public bool TryFindProvider(string hapticName, out IHapticProvider provider)
        {
            foreach (var item in produceList)
            {
                if (item.NameID == hapticName)
                {
                    provider = item;
                    return true;
                }
            }
            provider = null;
            return false;
        }
        public IHapticProvider GetProvider(int index)
        {
            return produceList[index];
        }

        // Abstract Methods
        protected abstract void OnPlay(HapticConfig config);
        protected abstract void OnPause(HapticConfig config);
        protected abstract void OnStop(HapticConfig config);
    }

    public abstract class HapticConfig : ConfigAsset, IIdentifier
    {
        [Header("Info")]
        [SerializeField]
        private string configName;

        // Implement 
        public string NameID => configName;


#if UNITY_EDITOR
        public void ChangeName(string newName)
        {
            configName = newName;
        }
#endif
    }
}