using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public interface IStat
    {
        string Name { get; }
        float BaseValue { get; }
        float CurrentValue { get; }
        float MinValue { get; }
        float MaxValue { get; }
    }
    public interface IModifiableStat : IStat
    {
        void AddModifier(IStatModifier modifier);
        void RemoveModifier(IStatModifier modifier);
        float GetFinalValue(); // Base + modifiers
    }
    public interface IStatModifier
    {
        float Value { get; }
        StatModifierType Type { get; } // e.g., Flat, PercentAdd, PercentMult
        int Priority { get; } // Order of application
    }
    public enum StatModifierType
    {
        Flat,
        PercentAdd,
        PercentMult
    }
    
    public class GameStat : IModifiableStat
    {
        private string statName;
        [SerializeField]
        private float baseValue;
        [SerializeField]
        private float currentValue;
        [SerializeField]
        private float minValue;
        [SerializeField]
        private float maxValue;
        private List<IStatModifier> modifiers = new();


        public GameStat(string name)
        {
            statName = name;
        }
        public GameStat(string name, float Value)
        {
            statName = name;
            baseValue = Value;
            currentValue = Value;
        }
        public GameStat(string name, float Value, float min, float max)
        {
            statName = name;
            baseValue = Value;
            currentValue = Value;
            minValue = min;
            maxValue = max;
        }


        // Implement IModifiableStat Interfaace
        string IStat.Name => statName;
        float IStat.BaseValue => baseValue;
        float IStat.CurrentValue => currentValue;
        float IStat.MinValue => minValue;
        float IStat.MaxValue => maxValue;
        public void AddModifier(IStatModifier mod) => modifiers.Add(mod);
        public void RemoveModifier(IStatModifier mod) => modifiers.Remove(mod);

        public float GetFinalValue()
        {
            float value = baseValue;
            float sumPercentAdd = 0f;

            foreach (var mod in modifiers)
            {
                switch (mod.Type)
                {
                    case StatModifierType.Flat:
                        value += mod.Value;
                        break;
                    case StatModifierType.PercentAdd:
                        sumPercentAdd += mod.Value;
                        break;
                    case StatModifierType.PercentMult:
                        value *= 1 + mod.Value;
                        break;
                }
            }
            value *= 1 + sumPercentAdd;
            value = Math.Clamp(value, minValue, maxValue);
            return value;
        }
    }

}