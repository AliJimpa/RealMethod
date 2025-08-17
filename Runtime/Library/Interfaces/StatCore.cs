
namespace RealMethod
{
    public interface IStat : IIdentifier
    {
        float BaseValue { get; }
        float Value { get; }
        float MinValue { get; }
        float MaxValue { get; }
        void BindNotify(System.Action<IStat> handler);
        void UnbindNotify(System.Action<IStat> handler);
    }

    public interface IModifiableStat
    {
        bool IsModifierValid(IStatModifier modifier);
        void AddModifier(IStatModifier modifier);
        //void UpdateModifier(IStatModifier modifier);
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
        //void UpdateValue(float newValue);
    }

}