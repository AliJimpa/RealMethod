using UnityEngine;

namespace RealMethod
{
    public static class Component_Extension
    {
        public static void SendSpawnEvent(this Component owner, Object spawner, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            owner.SendMessage("OnSpawn", spawner, option);
        }
        public static void SendDespawnEvent(this Component owner, Object despawner, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            owner.SendMessage("OnDespawn", despawner, option);
        }
    }
}