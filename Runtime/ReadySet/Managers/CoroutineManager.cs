namespace RealMethod
{
    public class CoroutineManager : EnumeratorManager
    {
        // EnumeratorManager Methods
        protected override void InitiateManager(bool alwaysLoaded)
        {
            if (Game.TryGetService(out Spawn SpawnServ))
            {
                SpawnServ.BringManager(this);
            }
        }
        protected override void InitiateService(Service service)
        {
            if (service is Spawn spawnservice)
            {
                spawnservice.BringManager(this);
            }
        }

    }
}