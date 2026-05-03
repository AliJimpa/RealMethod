using System;

namespace RealMethod
{
    public sealed class DefaultGameBridge : GameBridge
    {
        public DefaultGameBridge(Action<World> method) : base(method)
        {
        }
    }
}