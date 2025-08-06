using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public interface IStatContainer
    {
        IStat GetStat(int index);
        BaseStatData GetStatData(int index);
        void ApplyBuff(BuffConfig config);
        void DeclineBuff(BuffConfig config);
    }
    public interface IStatStorage : IStorage
    {
        void StoreStats(IStat[] stat);
        bool TryLoadStats(StatData data);
    }

    public abstract class StatProfile : DataAsset, IIdentifier, IStatContainer
    {
        [Header("Profile")]
        [SerializeField]
        private string profileName;
        [SerializeField, Tooltip("No Runtine effect")]
        private bool clamp = true;
        public bool Clamp => clamp;
        [SerializeField, Tooltip("No Runtine effect")]
        private bool sortPriority;
        public bool SortPriority => sortPriority;
        public int Count => GetStatCount();

        public IStat this[int index]
        {
            get
            {
                return GetStat(index);
            }
        }

        // Implement IIdentifier Interface
        public string NameID => profileName;

        // Abstract Method
        public abstract IStat GetStat(int index);
        public abstract BaseStatData GetStatData(int index);
        public abstract void ApplyBuff(BuffConfig config);
        public abstract void DeclineBuff(BuffConfig config);
        protected abstract int GetStatCount();

#if UNITY_EDITOR
        public void ChangeName(string NewName)
        {
            profileName = NewName;
        }
#endif
    }
    public abstract class StatProfile<En, Sd> : StatProfile where En : System.Enum where Sd : StatData
    {
        [System.Serializable]
        private class GameStat : SerializableDictionary<En, Sd> { }
        [Header("Stat")]
        [SerializeField]
        private GameStat ChacterStats;
        protected IStatStorage Storage { get; private set; }


        // StatContainer Methods
        public sealed override IStat GetStat(int index)
        {
            CheckStorage();
            int i = 0;
            foreach (var stat in ChacterStats)
            {
                if (i == index)
                {
                    return stat.Value;
                }
                i++;
            }
            return null;
        }
        public sealed override BaseStatData GetStatData(int index)
        {
            CheckStorage();
            int i = 0;
            foreach (var stat in ChacterStats)
            {
                if (i == index)
                {
                    return stat.Value;
                }
                i++;
            }
            return null;
        }
        protected sealed override int GetStatCount()
        {
            return ChacterStats != null ? ChacterStats.Count : 0;
        }
        public sealed override void ApplyBuff(BuffConfig config)
        {
            CheckStorage();
            foreach (var stat in ChacterStats)
            {
                foreach (var modf in config.GetModifiers(stat.Key))
                {
                    ((IModifiableStat)stat.Value).AddModifier(modf);
                }
            }
        }
        public sealed override void DeclineBuff(BuffConfig config)
        {
            CheckStorage();
            foreach (var stat in ChacterStats)
            {
                foreach (var modf in config.GetModifiers(stat.Key))
                {
                    ((IModifiableStat)stat.Value).RemoveModifier(modf);
                }
            }
        }


        // Public Functions
        public En[] GetStatList()
        {
            return ChacterStats.Keys.ToArray();
        }
        public IStat GetStat(En identity)
        {
            CheckStorage();
            return ChacterStats[identity];
        }

        // Private Functions
        private void CheckStorage()
        {
            if (Storage == null)
            {
                // First Touch
                Storage = GetStorage();
                if (Storage != null)
                {
                    if (LoadStorage())
                    {
                        foreach (var stat in ChacterStats)
                        {
                            ((IStatDatainitializer)stat.Value).SetName(NameID + "_" + stat.Key.ToString());
                            if (Storage.TryLoadStats(stat.Value))
                            {
                                ((IStatDatainitializer)stat.Value).initializer(this, true);
                            }
                            else
                            {
                                ((IStatDatainitializer)stat.Value).initializer(this, false);
                                Debug.LogWarning($"This {stat.Key} Stat is not any load Value");
                            }
                        }
                    }
                    else
                    {
                        foreach (var stat in ChacterStats)
                        {
                            ((IStatDatainitializer)stat.Value).SetName(NameID + "_" + stat.Key.ToString());
                            ((IStatDatainitializer)stat.Value).initializer(this, false);
                        }
                        Storage.StoreStats(ChacterStats.Values.ToArray());
                    }
                }
                else
                {
                    Debug.LogWarning("Storage is Not Valid");
                }
            }
        }


        // Abstract Method
        protected abstract IStatStorage GetStorage();
        protected abstract bool LoadStorage();
    }
    public abstract class StatProfileStorage<En, Sd> : StatProfile<En, Sd> where En : System.Enum where Sd : StatData
    {
        [Header("Save")]
        [SerializeField]
        private StorageFile<IStatStorage, StatSaveFile> storage;
        public SaveFile file => storage.file;

        protected sealed override IStatStorage GetStorage()
        {
            return storage.provider;
        }
        protected sealed override bool LoadStorage()
        {
            return storage.Load(this);
        }
    }

}