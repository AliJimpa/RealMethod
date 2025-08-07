namespace RealMethod
{
    public interface ICooldown
    {
        float CooldownDuration { get; }

        bool IsAvailable { get; }
        void ResetCooldown();
        void StartCooldown();

    }

    public interface ICooldownTick : ICooldown, ITick
    {
        float CooldownRemaining { get; }
    }

}