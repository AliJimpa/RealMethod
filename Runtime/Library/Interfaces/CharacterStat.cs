using System.Runtime.CompilerServices;

namespace RealMethod
{
    public interface IStat : IIdentifier
    {
        float BaseValue { get; }
        float Value { get; }
        float MinValue { get; }
        float MaxValue { get; }
    }

    public interface IModifiableStat
    {
        void AddModifier(IStatModifier modifier);
        void RemoveModifier(IStatModifier modifier);
    }

    public interface IStatModifier
    {
        public enum StatUnitModifierType
        {
            Flat,
            PercentAdd,
            PercentMult
        }
        float Value { get; }
        StatUnitModifierType Type { get; } // e.g., Flat, PercentAdd, PercentMult
        int Priority { get; } // Order of application
        void UpdateValue(float newValue);
    }

    public interface IStatInitiator
    {
        void Initialize();
    }


}