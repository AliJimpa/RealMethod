namespace RealMethod
{
    public interface ICooldown
    {
        float CooldownDuration { get; }
        float CooldownRemaining { get; }

        bool IsAvailable { get; }
        void StartCooldown();
        void TickCooldown(float deltaTime);
    }
}