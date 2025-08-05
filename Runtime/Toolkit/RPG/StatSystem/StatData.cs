using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public interface IStatDatainitializer
    {
        void SetName(string statName);
        void initializer(StatProfile profile, bool isloaded);
    }

    public abstract class BaseStatData : IStat, IModifiableStat
    {
        [SerializeField]
        private float baseValue;
        [SerializeField, ReadOnly]
        private float currentValue;
        [SerializeField]
        private float minValue;
        [SerializeField]
        private float maxValue;

        public System.Action<BaseStatData> OnChangeValue;

        private List<IStatModifier> modifiers = new List<IStatModifier>(5);
        public int modifiersCount => modifiers.Count;
        private bool isDirty = false;

        public BaseStatData()
        {

        }
        public BaseStatData(float startValue, float min, float max)
        {
            baseValue = startValue;
            minValue = min;
            maxValue = max;
        }


        // Implement IStat Interface
        public string NameID => GetStatName();
        public float BaseValue => baseValue;
        public float Value => GetFinalValue();
        public float MinValue => minValue;
        public float MaxValue => maxValue;
        // Implement IModifiableStat Interface
        void IModifiableStat.AddModifier(IStatModifier mod)
        {
            modifiers.Add(mod);
            isDirty = true;
            OnChangeValue?.Invoke(this);
        }
        void IModifiableStat.RemoveModifier(IStatModifier mod)
        {
            modifiers.Remove(mod);
            isDirty = true;
            OnChangeValue?.Invoke(this);
        }

        // Public Functions
        public void SetValue(float value)
        {
            baseValue = value;
        }
        public void SetLimitation(float min, float max)
        {
            minValue = min;
            maxValue = max;
        }

        // Private Functions
        private float Clamp(float value)
        {
            return CanClamp() ? System.Math.Clamp(value, minValue, maxValue) : value;
        }
        private float GetFinalValue()
        {
            if (isDirty)
            {
                float finalValue = baseValue;
                float sumPercentAdd = 0f;
                // Sort by Priority before applying modifiers
                IEnumerable<IStatModifier> modifiersToApply = CanSort() ? modifiers.OrderBy(m => m.Priority) : modifiers;
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
        protected abstract string GetStatName();
        protected abstract bool CheckModifier(IStatModifier mod);
        protected abstract bool CanClamp();
        protected abstract bool CanSort();
    }
    public abstract class StatData : BaseStatData, IStatDatainitializer
    {
        private string MyName = string.Empty;
        public bool IsAllowedClamp { get; protected set; }
        public bool IsAllowedSort { get; protected set; }



        // BaseStatData Methods
        protected sealed override string GetStatName()
        {
            return MyName;
        }
        protected sealed override bool CanClamp()
        {
            return IsAllowedClamp;
        }
        protected sealed override bool CanSort()
        {
            return IsAllowedSort;
        }

        // Implement IStatDatainitializer Interface
        void IStatDatainitializer.SetName(string statName)
        {
            MyName = statName;
        }
        void IStatDatainitializer.initializer(StatProfile profile, bool isloaded)
        {
            IsAllowedClamp = profile.Clamp;
            IsAllowedSort = profile.SortPriority;
            if (isloaded)
                OnLoaded();
            OnInitiate(profile);
        }

        // Abstract Method
        protected abstract void OnLoaded();
        protected abstract void OnInitiate(StatProfile profile);


    }

}