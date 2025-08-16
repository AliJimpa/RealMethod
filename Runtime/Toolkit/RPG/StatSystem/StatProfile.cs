using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public interface IPrimitiveStatContainer
    {
        IStat GetStat(int index);
        void ApplyBuff(BuffConfig config);
        void DeclineBuff(BuffConfig config);
        void InitializeResource(IResourceData resource);
    }
    public interface IPrimitiveStatContainer<T> : IPrimitiveStatContainer where T : System.Enum
    {
        void ApplyBuff(IStatModifier modifier, T identity);
        void DeclineBuff(IStatModifier modifier, T identity);
        IStat GetStat(T identity);
    }
    public interface IStatStorage : IStorage
    {
        void StoreStats(IStat[] stat);
        bool TryLoadStats(StatData data);
    }

    public abstract class StatProfile : DataAsset, IIdentifier, IPrimitiveStatContainer
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
        void IPrimitiveStatContainer.InitializeResource(IResourceData resource)
        {
            resource.Initialize(this);
        }

        // Abstract Method
        public abstract IStat GetStat(int index);
        public abstract IStat GetStat(string name);
        public abstract BaseStatData GetStatData(int index);
        public abstract void ApplyBuff(BuffConfig config);
        public abstract void DeclineBuff(BuffConfig config);
        public abstract string[] GetStatNames();
        public abstract void StoreStats();
        protected abstract int GetStatCount();
        protected abstract IStatStorage GetStorage();
        protected abstract bool LoadStorage();

#if UNITY_EDITOR
        public void ChangeName(string NewName)
        {
            profileName = NewName;
        }
#endif
    }
    public abstract class StatProfileStorage : StatProfile
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
    public abstract class StatProfile<En, Sd> : StatProfileStorage, IPrimitiveStatContainer<En> where En : System.Enum where Sd : StatData
    {
        [System.Serializable]
        private class GameStat : SerializableDictionary<En, Sd> { }
        [Header("Definition")]
        [SerializeField]
        private GameStat ChacterStats;
        protected IStatStorage Storage { get; private set; }

        public Sd this[En identity]
        {
            get
            {
                return ChacterStats[identity];
            }
        }

        // StatProfileStorage Methods
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
        public sealed override IStat GetStat(string name)
        {
            CheckStorage();
            foreach (var stat in ChacterStats)
            {
                if (stat.Key.ToString() == name)
                {
                    return stat.Value;
                }
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
        public void ApplyBuff(IStatModifier modifier)
        {
            CheckStorage();
            ((IModifiableStat)ChacterStats[stat]).AddModifier(modifier);
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
        public void DeclineBuff(IStatModifier modifier)
        {
            CheckStorage();
            ((IModifiableStat)ChacterStats[stat]).RemoveModifier(modifier);
        }
        public sealed override string[] GetStatNames()
        {
            string[] result = new string[ChacterStats.Count];
            for (int i = 0; i < ChacterStats.Count; i++)
            {
                result[i] = ChacterStats.ElementAt(i).Key.ToString();
            }
            return result;
        }
        public sealed override void StoreStats()
        {
            CheckStorage();
            Storage.StoreStats(ChacterStats.Values.ToArray());
        }
        protected sealed override int GetStatCount()
        {
            return ChacterStats != null ? ChacterStats.Count : 0;
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
        public void Clear()
        {
            foreach (var stat in ChacterStats)
            {
                stat.Value.Clear();
            }
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
                            ((IStatDataInitializer)stat.Value).SetName(NameID + "_" + stat.Key.ToString());
                            if (Storage.TryLoadStats(stat.Value))
                            {
                                ((IStatDataInitializer)stat.Value).Initializer(this, true);
                            }
                            else
                            {
                                ((IStatDataInitializer)stat.Value).Initializer(this, false);
                                Debug.LogWarning($"This {stat.Key} Stat is not any load Value");
                            }
                        }
                    }
                    else
                    {
                        foreach (var stat in ChacterStats)
                        {
                            ((IStatDataInitializer)stat.Value).SetName(NameID + "_" + stat.Key.ToString());
                            ((IStatDataInitializer)stat.Value).Initializer(this, false);
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


#if UNITY_EDITOR
        public override void OnEditorPlay()
        {
            Clear();
        }
#endif
    }

}