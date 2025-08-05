using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public interface IStatStorage : IStorage
    {
        void StartStats(IStat[] stat);
        bool TryLoadStats(StatData data);


        IStatModifier CreateModifier(int StatIndex, IIdentifier config, float value, IStatModifier.StatUnitModifierType type, int priority);
        IStatModifier[] GetModifiers(int StatIndex);
        void RemoveModifier(int StatIndex, IIdentifier config, IModifiableStat StatData);
    }

    public abstract class StatProfile : DataAsset, IIdentifier
    {
        [Header("Profile")]
        private string profileName;
        [SerializeField]
        private bool clamp;
        public bool Clamp => clamp;
        [SerializeField]
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
        protected abstract int GetStatCount();
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
        public void ApplyBuff(StatModifierConfig config)
        {
            CheckStorage();
            foreach (var stat in ChacterStats)
            {
                IStatModifier[] result = config.MakeModifiers(stat.Key, Storage);
                foreach (var modf in result)
                {
                    ((IModifiableStat)stat.Value).AddModifier(modf);
                }
            }
        }
        public void DeclineBuff(StatModifierConfig config)
        {
            CheckStorage();
            foreach (var stat in ChacterStats)
            {
                int statindex = System.Convert.ToInt32(stat.Key);
                Storage.RemoveModifier(statindex, config, stat.Value);
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
                            ((IStatDatainitializer)stat.Value).SetName(NameID + "_" + stat.Key.ToString());
                            if (Storage.TryLoadStats(stat.Value))
                            {
                                IStatModifier[] statmodifiers = Storage.GetModifiers(System.Convert.ToInt32(stat.Key));
                                foreach (var modif in statmodifiers)
                                {
                                    ((IModifiableStat)stat.Value).AddModifier(modif);
                                }
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
                        Storage.StartStats(ChacterStats.Values.ToArray());
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