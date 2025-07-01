namespace RealMethod
{
    public enum UpdateMethod
    {
        None = 0,
        LateUpdate = 2,
        Update = 3,
        FixedUpdate = 4

    }
    public enum LoadStage
    {
        Ascync,
        Scync
    }
    public enum TriggerStage
    {
        None = 0,
        Enter = 1,
        Exit = 2,
    }

}