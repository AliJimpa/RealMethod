namespace RealMethod
{
    public enum UpdateMethod
    {
        None = 0,
        LateUpdate = 2,
        Update = 3,
        FixedUpdate = 4

    }
    public enum TriggerStage
    {
        None = 0,
        Enter = 1,
        Stay = 2,
        Exit = 2,
    }
    public enum InputSystemType
    {
        OldInputSystem,
        NewInputSystem
    }
}