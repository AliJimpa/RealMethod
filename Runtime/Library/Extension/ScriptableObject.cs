using System.Reflection;
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
        public static void InvokeSpawnEvent(this ScriptableObject owner, Object spawner = null)
        {
            if (spawner != null)
            {
                if (owner is ISpawnWithAuthor provider)
                {
                    provider.OnSpawn(spawner);
                }
                else
                {
                    owner.Invoke(GameMessage.Spawn, new object[1] { spawner });
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
                    owner.Invoke(GameMessage.Spawn);
                }
            }
        }
        /// <summary>
        /// Sends an "OnDespawn" message to the owner GameObject.
        /// </summary>
        /// <param name="owner">The GameObject to send the message to.</param>
        /// <param name="despawner">The object that triggered the despawn event (passed as parameter to the message).</param>
        public static void InvokeDespawnEvent(this ScriptableObject owner, Object despawner = null)
        {
            if (despawner != null)
            {
                if (owner is IDespawnWithAuthor provider)
                {
                    provider.OnDespawn(despawner);
                }
                else
                {
                    owner.Invoke(GameMessage.Despawn, new object[1] { despawner });
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
                    owner.Invoke(GameMessage.Despawn);
                }
            }
        }

        public static void Invoke(this ScriptableObject so, string methodName)
        {
            so.Invoke(methodName, null);
        }
        public static void Invoke(this ScriptableObject so, string methodName, object[] parameters)
        {
            var method = so.GetType().GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            if (method != null)
            {
                method.Invoke(so, parameters);
            }
            else
            {
                Debug.LogError($"Method '{methodName}' not found.");
            }
        }
        public static ScriptableObject Clone(this ScriptableObject so)
        {
            return ScriptableObject.Instantiate(so);
        }

    }
}