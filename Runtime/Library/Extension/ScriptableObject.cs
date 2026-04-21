using UnityEngine;

namespace RealMethod
{
    public static class ScriptableObject_Extension
    {
        /// <summary>
        /// Sends an "OnSpawn" message to the owner GameObject.
        /// </summary>
        /// <param name="owner">The GameObject to send the message to.</param>
        /// <param name="spawner">The object that triggered the spawn event (passed as parameter to the message).</param>
        public static void InvokeSpawnEvent(this ScriptableObject owner, Object spawner = null, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            if (spawner != null)
            {
                if (owner is ISpawnWithAuthor provider)
                {
                    provider.OnSpawn(spawner);
                }
                else
                {
                    owner.SendMessage(MessageNames.Spawn, spawner, option);
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
                    owner.SendMessage(MessageNames.Spawn, option);
                }
            }
        }
        /// <summary>
        /// Sends an "OnDespawn" message to the owner GameObject.
        /// </summary>
        /// <param name="owner">The GameObject to send the message to.</param>
        /// <param name="despawner">The object that triggered the despawn event (passed as parameter to the message).</param>
        public static void InvokeDespawnEvent(this ScriptableObject owner, Object despawner = null, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            if (despawner != null)
            {
                if (owner is IDespawnWithAuthor provider)
                {
                    provider.OnDespawn(despawner);
                }
                else
                {
                    owner.SendMessage(MessageNames.Despawn, despawner, option);
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
                    owner.SendMessage(MessageNames.Despawn, option);
                }
            }
        }
        public static ScriptableObject Clone(this ScriptableObject so)
        {
            return ScriptableObject.Instantiate(so);
        }

    }
}