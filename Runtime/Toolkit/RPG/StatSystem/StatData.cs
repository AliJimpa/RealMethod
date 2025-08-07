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
        private float firstValue;
        public float FirstValue => firstValue;
        [SerializeField, ReadOnly]
        private float extraValue;
        [SerializeField, ReadOnly]
        private float currentValue;
        [SerializeField]
        private float minValue;
        [SerializeField]
        private float maxValue;

        private System.Action<IStat> OnChangeValue;

        private List<IStatModifier> modifiers = new List<IStatModifier>(5);
        public int modifiersCount => modifiers.Count;
        private bool isDirty = true;

        public BaseStatData()
        {

        }
        public BaseStatData(float startValue, float min, float max)
        {
            firstValue = startValue;
            minValue = min;
            maxValue = max;
        }


        // Implement IStat Interface
        public string NameID => GetStatName();
        public float BaseValue => firstValue + extraValue;
        public float Value => GetFinalValue();
        public float MinValue => minValue;
        public float MaxValue => maxValue;
        public void BindNotify(System.Action<IStat> handler)
        {
            OnChangeValue += handler;
        }
        public void UnbindNotify(System.Action<IStat> handler)
        {
            OnChangeValue -= handler;
        }
        // Implement IModifiableStat Interface
        void IModifiableStat.AddModifier(IStatModifier mod)
        {
            if (!modifiers.Contains(mod))
            {
                modifiers.Add(mod);
                isDirty = true;
                OnChangeValue?.Invoke(this);
            }
            else
            {
                Debug.LogWarning("The Modifier already added!");
            }
        }
        void IModifiableStat.RemoveModifier(IStatModifier mod)
        {
            modifiers.Remove(mod);
            isDirty = true;
            OnChangeValue?.Invoke(this);
        }

        // Public Functions
        public void SetExtraValue(float value)
        {
            extraValue = value;
            isDirty = true;
            OnChangeValue?.Invoke(this);
        }
        public void SetLimitation(float min, float max)
        {
            minValue = min;
            maxValue = max;
            isDirty = true;
            OnChangeValue?.Invoke(this);
        }
        public void Clear()
        {
            modifiers.Clear();
            extraValue = 0;
            currentValue = 0;
            isDirty = true;
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
                float finalValue = BaseValue;
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