namespace RealMethod
{
    public interface IStatStorage : IStorage
    {
        IStatModifier CreateModifier(IStat stat, IIdentifier ModiferID, int ModiferIndex, float value, IStatModifier.StatUnitModifierType type, int priority);
        IStatModifier GetModifier(IIdentifier ModiferID, int ModiferIndex);
        void RemoveModifier(IIdentifier ModiferID, int ModiferIndex);
    }
}