using UnityEngine;

namespace RealMethod
{
    public sealed class DefaultGameService : GameService
    {
        public override void Start(object Author)
        {
            Debug.Log("DefaultGameService Started");
        }
        public override void WorldUpdated()
        {
        }
        protected override void NewWorld(World target)
        {
        }
        protected override void NewAdditiveWorld(World target)
        {
        }
        public override void End(object Author)
        {
            Debug.LogError("DefaultGameService Ended [This should not happent]");
        }
    }
}