using UnityEngine;

namespace RealMethod
{
    public static class Component_Extension
    {
        public static void InvokeSpawnEvent(this Component owner, Object spawner = null, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            if (spawner != null)
            {
                if (owner is ISpawnWithAuthor provider)
                {
                    provider.OnSpawn(spawner);
                }
                else
                {
                    owner.SendMessage(GameMessage.Spawn, spawner, option);
                }
            }
            else
            {
                if (owner is ISpawn provider)
                {
                    provider.OnSpawn();
                }
                else
                {
                    owner.SendMessage(GameMessage.Spawn, option);
                }
            }
        }
        public static void InvokeDespawnEvent(this Component owner, Object despawner = null, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            if (despawner != null)
            {
                if (owner is IDespawnWithAuthor provider)
                {
                    provider.OnDespawn(despawner);
                }
                else
                {
                    owner.SendMessage(GameMessage.Despawn, despawner, option);
                }
            }
            else
            {
                if (owner is IDespawn provider)
                {
                    provider.OnDespawn();
                }
                else
                {
                    owner.SendMessage(GameMessage.Despawn, option);
                }
            }
        }
    }
}