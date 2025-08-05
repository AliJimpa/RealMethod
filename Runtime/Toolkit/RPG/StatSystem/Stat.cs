using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public abstract class StatContainer : MonoBehaviour
    {
        [Header("Setting")]
        [SerializeField]
        private StatAsset[] DefaultStat;

        private Hictionary<IStat> Stats;
        public int Count => Stats.Count;

        public IStat this[string Name]
        {
            get
            {
                return Stats[Name];
            }
        }

        private void Awake()
        {
            if (DefaultStat != null)
            {
                Stats = new Hictionary<IStat>(DefaultStat.Length + 5);
                foreach (var Nstat in DefaultStat)
                {
                    Stats.Add(Nstat.NameID, Nstat);
                    OnStatAdded(Nstat);
                }
            }
            else
            {
                Stats = new Hictionary<IStat>(5);
            }
        }

        // Public Functions
        public bool IsValid(string nameStat)
        {
            return Stats.ContainsKey(nameStat);
        }
        public bool TryFindStat(string nameStat, out IStat provider)
        {
            if (Stats.ContainsKey(nameStat))
            {
                provider = Stats[nameStat];
                return true;
            }
            else
            {
                provider = null;
                return false;
            }
        }
        public void Add(IStat stat)
        {
            Stats.Add(stat.NameID, stat);
            OnStatAdded(stat);
            SendMessage("OnAddStat", stat, SendMessageOptions.DontRequireReceiver);
        }
        public bool Remove(string nameStat)
        {
            if (Stats.ContainsKey(nameStat))
            {
                Stats.Remove(nameStat);
                OnStatRemoved(nameStat);
                SendMessage("OnRemoveStat", nameStat, SendMessageOptions.DontRequireReceiver);
                return true;
            }
            else
            {
                return false;
            }
        }

        // Abstract Method
        protected abstract void OnStatAdded(IStat stat);
        protected abstract void OnStatRemoved(string statName);
    }


    public abstract class StatAsset : DataAsset, IStat, IModifiableStat
    {
        [Header("StatAsset")]
        [SerializeField]
        private string statName;
        [SerializeField]
        private float baseValue;
        [SerializeField, ReadOnly]
        private float currentValue;
        [SerializeField]
        private float minValue;
        [SerializeField]
        private float maxValue;
        [SerializeField]
        private bool clamp = true;
        [SerializeField]
        private bool sortByPriority = false;


        public System.Action<StatAsset> OnStatChange;


        private List<IStatModifier> modifiers = new List<IStatModifier>(5);
        public int modifiersCount => modifiers.Count;
        protected IStatStorage Storage { get; private set; }
        private bool isDirty = false;


        // Implement IStat Interface
        public string NameID => statName;
        public float BaseValue => baseValue;
        public float Value => GetFinalValue();
        public float MinValue => minValue;
        public float MaxValue => maxValue;
        // Implement IModifiableStat Interface
        void IModifiableStat.AddModifier(IStatModifier mod)
        {
            modifiers.Add(mod);
            isDirty = true;
            OnValueChanged();
            OnStatChange?.Invoke(this);
        }
        void IModifiableStat.RemoveModifier(IStatModifier mod)
        {
            modifiers.Remove(mod);
            isDirty = true;
            OnValueChanged();
            OnStatChange?.Invoke(this);
        }


        // Public Functions
        public void SetValue(float value)
        {
            baseValue = value;
        }
        public void SetActiveClamp(bool active)
        {
            clamp = active;
        }
        public void SetLimitation(float min, float max)
        {
            minValue = min;
            maxValue = max;
        }

        // Private Functions
        private float Clamp(float value)
        {
            return clamp ? System.Math.Clamp(value, minValue, maxValue) : value;
        }
        private float GetFinalValue()
        {
            if (Storage == null)
            {
                // First Touch
                Storage = GetStorage();
                if (Storage != null)
                {
                    if (LoadStorage())
                    {

                    }
                    else
                    {

                    }
                }
                else
                {
                    Debug.LogWarning("Storage is Not Valid");
                }
            }

            if (isDirty)
            {
                float finalValue = baseValue;
                float sumPercentAdd = 0f;
                // Sort by Priority before applying modifiers
                IEnumerable<IStatModifier> modifiersToApply = sortByPriority ? modifiers.OrderBy(m => m.Priority) : modifiers;
                foreach (var mod in modifiersToApply)
                {
                    if (CheckModifier(mod))
                    {
                        switch (mod.Type)
                        {
                            case IStatModifier.StatUnitModifierType.Flat:
                                finalValue += mod.Value;
                                break;

                            case IStatModifier.StatUnitModifierType.PercentAdd:
                                sumPercentAdd += mod.Value;
                                break;

                            case IStatModifier.StatUnitModifierType.PercentMult:
                                finalValue *= 1 + mod.Value;
                                break;
                        }
                    }
                }
                // Apply PercentAdd *after* all Flat modifiers
                if (sumPercentAdd != 0)
                {
                    finalValue *= 1 + sumPercentAdd;
                }

                currentValue = Clamp(finalValue);
                isDirty = false;
            }
            return currentValue;
        }

        // Abstract Method
        protected abstract bool CheckModifier(IStatModifier mod);
        protected abstract void OnValueChanged();
        protected abstract IStatStorage GetStorage();
        protected abstract bool LoadStorage();
    }
    public abstract class StatStorage : StatAsset
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


    public abstract class StatModifierConfig : ConfigAsset, IIdentifier
    {
        [System.Serializable]
        private struct StatModifierPreset
        {
            [SerializeField]
            private string statName;
            public string StatName => statName;
            [SerializeField]
            private float value;
            public float Value => value;
            [SerializeField]
            private IStatModifier.StatUnitModifierType type;
            public IStatModifier.StatUnitModifierType Type => type;
            [SerializeField]
            private int priority;
            public int Priority => priority;
        }
        [Header("StatConfig")]
        [SerializeField]
        private string configName;
        [SerializeField]
        private StatModifierPreset[] modifiers;
        public int Count => modifiers != null ? modifiers.Length : 0;

        // Implement IIdentifier Interface
        public string NameID => configName;

        public Dictionary<int, string> GetModifierSets()
        {
            Dictionary<int, string> Result = new Dictionary<int, string>(modifiers.Length);
            for (int i = 0; i < modifiers.Length; i++)
            {
                Result.Add(i, modifiers[i].StatName);
            }
            return Result;
        }
        public void Apply(int index, StatAsset asset, IStatStorage storage)
        {
            IStatModifier modif = storage.CreateModifier(asset, this, index, modifiers[index].Value, modifiers[index].Type, modifiers[index].Priority);
            ((IModifiableStat)asset).AddModifier(modif);
        }
        public void Decline(int index, StatAsset asset, IStatStorage storage)
        {
            IStatModifier modif = storage.GetModifier(this, index);
            if (modif != null)
            {
                ((IModifiableStat)asset).RemoveModifier(modif);
                storage.RemoveModifier(this, index);
            }
            else
            {
                Debug.LogWarning($"Can't Find any Modifer in storage for Name: {configName} in Index: {index}");
            }
        }
    }


}