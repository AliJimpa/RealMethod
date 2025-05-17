namespace RealMethod
{
    public class DefaultGameService : GameService
    {
        public override void Start(object Author) { }
        public override void WorldUpdated() { }
        protected override void NewWorld(World target) { }
        protected override void NewAdditiveWorld(World target) { }
        public override void End(object Author) { }
    }
}