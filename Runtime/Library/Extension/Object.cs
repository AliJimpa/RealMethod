using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    public static class Object_Extension
    {
        public static bool NullChecker(this object Obj, string Name)
        {
            if (Obj != null)
            {
                return true;
            }
            else
            {
                Debug.LogError($"Target Object [{Name}] Is Not Valid");
                return false;
            }
        }
        /// <summary>
        /// Sends an "OnSpawn" message to the owner object.
        /// Auto detect type for MonoBehaviour,GameObject,PrimitiveAsset and object
        /// </summary>
        /// <param name="owner">The GameObject to send the message to.</param>
        /// <param name="spawner">The object that triggered the spawn event (passed as parameter to the message).</param>
        /// <param name="option">Specifies whether the message must be received, or if it is optional.</param>
        public static void InvokeSpawnEvent(this object owner, Object spawner = null, SendMessageOptions option = SendMessageOptions.DontRequireReceiver)
        {
            if (owner is MonoBehaviour mono)
            {
                if (spawner != null)
                {
                    if (mono is ISpawnWithAuthor provider)
                    {
                        provider.OnSpawn(spawner);
                    }
                    else
                    {
                        mono.SendMessage(MessageNames.Spawn, spawner, option);
                    }
                }
                else
                {
                    if (mono is ISpawn provider)
                    {
                        provider.OnSpawn();
                    }
                    else
                    {
                        mono.SendMessage(MessageNames.Spawn, option);
                    }
                }
            }
            else if (owner is GameObject gameobject)
            {
                if (spawner != null)
                {
                    ISpawnWithAuthor provider = gameobject.GetComponent<ISpawnWithAuthor>();
                    if (provider != null)
                    {
                        provider.OnSpawn(spawner);
                    }
                    else
                    {
                        gameobject.SendMessage(MessageNames.Spawn, spawner, option);
                    }
                }
                else
                {
                    ISpawn provider = gameobject.GetComponent<ISpawn>();
                    if (provider != null)
                    {
                        provider.OnSpawn();
                    }
                    else
                    {
                        gameobject.SendMessage(MessageNames.Spawn, option);
                    }
                }
            }
            else
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
        }
        /// <summary>
        /// Sends an "OnDespawn" message to the owner object.
        /// Auto detect type for MonoBehaviour,GameObject,PrimitiveAsset and object
        /// </summary>
        /// <param name="owner">The GameObject to send the message to.</param>
        /// <param name="despawner">The object that triggered the despawn event (passed as parameter to the message).</param>
        /// <param name="option">Specifies whether the message must be received, or if it is optional.</param>
        public static void InvokeDespawnEvent(this object owner, Object despawner = null, SendMessageOptions option = SendMessageOptions.DontRequireReceiver)
        {
            if (owner is MonoBehaviour mono)
            {
                if (despawner != null)
                {
                    if (mono is IDespawnWithAuthor provider)
                    {
                        provider.OnDespawn(despawner);
                    }
                    else
                    {
                        mono.SendMessage(MessageNames.Despawn, despawner, option);
                    }
                }
                else
                {
                    if (mono is IDespawn provider)
                    {
                        provider.OnDespawn();
                    }
                    else
                    {
                        mono.SendMessage(MessageNames.Despawn, option);
                    }
                }
            }
            else if (owner is GameObject gameobject)
            {
                if (despawner != null)
                {
                    IDespawnWithAuthor provider = gameobject.GetComponent<IDespawnWithAuthor>();
                    if (provider != null)
                    {
                        provider.OnDespawn(despawner);
                    }
                    else
                    {
                        gameobject.SendMessage(MessageNames.Despawn, despawner, option);
                    }
                }
                else
                {
                    IDespawn provider = gameobject.GetComponent<IDespawn>();
                    if (provider != null)
                    {
                        provider.OnDespawn();
                    }
                    else
                    {
                        gameobject.SendMessage(MessageNames.Despawn, option);
                    }
                }
            }
            else
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
        }
        public static void InvokeSaveEvent(this object obj, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            if (obj is ISaveable provider)
            {
                provider.OnSaved();
            }
            else
            {
                obj.SendMessage(MessageNames.Save, option);
            }
        }
        public static void InvokeLoadEvent(this object obj, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            if (obj is ISaveable provider)
            {
                provider.OnLoaded();
            }
            else
            {
                obj.SendMessage(MessageNames.Load, option);
            }
        }
        public static void InvokeReqisterEvent(this object obj, SendMessageOptions option = SendMessageOptions.DontRequireReceiver)
        {
            if (obj is IRegistrable provider)
            {
                provider.OnRegister();
            }
            else
            {
                obj.SendMessage(MessageNames.Register, option);
            }
        }
        public static void InvokeUnreqisterEvent(this object obj, SendMessageOptions option = SendMessageOptions.DontRequireReceiver)
        {
            if (obj is IRegistrable provider)
            {
                provider.OnUnregister();
            }
            else
            {
                obj.SendMessage(MessageNames.Unregister, option);
            }
        }
        public static void SendMessage(this object obj, string methodName, SendMessageOptions option)
        {
            obj.SendMessage(methodName, null, option);
        }
        public static void SendMessage(this object obj, string methodName, object parameter, SendMessageOptions options)
        {
            if (obj is Component comp)
            {
                comp.SendMessage(methodName, parameter, options);
                return;
            }

            var method = obj.GetType().GetMethodInHierarchy(methodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            if (method != null)
            {
                if (parameter == null)
                {
                    method.Invoke(obj, null);
                }
                else
                {
                    method.Invoke(obj, new object[1] { parameter });
                }
            }
            else
            {
                if (options == SendMessageOptions.RequireReceiver)
                    Debug.LogError($"[SendMessageError] Method '{methodName}' was not found on '{obj}'.");
            }
        }
        /// <summary>
        /// Attempts to cast the given <paramref name="obj"/> to the specified <paramref name="type"/> at runtime.
        /// </summary>
        /// <param name="obj">The source object to be cast.</param>
        /// <param name="type">The target type to cast to.</param>
        /// <returns>
        /// Return true is cansting correct
        /// </returns>
        public static bool IsChild(this object obj, System.Type type)
        {
            if (obj == null)
                throw new System.ArgumentNullException(nameof(obj));
            if (type == null)
                throw new System.ArgumentNullException(nameof(type));

            return type.IsInstanceOfType(obj);
        }
        public static T Cast<T>(this object obj) where T : class
        {
            return (T)obj;
        }
        public static bool TryCast<T>(this object obj, out T result) where T : class
        {
            if (obj is T refrence)
            {
                result = refrence;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        public static SoftType<T> GetSoftType<T>(this object obj) where T : System.Type
        {
            return obj.GetType();
        }
        public static bool TryGetType<T>(this object obj, out SoftType<T> result) where T : System.Type
        {
            System.Type targetType = obj.GetType();
            if (targetType.IsAssignableFrom(typeof(T)))
            {
                result = obj.GetType();
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        /// <summary>
        /// Checks whether the object contains a component that implements or matches type T.
        /// </summary>
        /// <typeparam name="T">The interface or component type to search for.</typeparam>
        /// <param name="target">The object to check.</param>
        /// <returns>True if a component of type T exists on the object; otherwise false.</returns>
        public static bool HasImplementInterface<T>(this object target)
        {
            if (target != null)
            {
                if (target is T provider)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks whether the object contains a component that implements or matches type T
        /// and returns the found component.
        /// </summary>
        /// <typeparam name="T">The interface or component type to search for.</typeparam>
        /// <param name="target">The object to check.</param>
        /// <param name="Result">Outputs the found component if one exists.</param>
        /// <returns>True if a component of type T exists on the object; otherwise false.</returns>
        public static bool HasImplementInterface<T>(this object target, out T Result)
        {
            if (target != null)
            {
                if (target is T provider)
                {
                    Result = provider;
                    return true;
                }
            }

            Result = default;
            return false;

        }
        /// <summary>
        /// Get all fields from object by filtering bindingflags
        /// </summary>
        /// <param name="target">the object refrence</param>
        /// <param name="flags">make filtering witch type of field</param>
        /// <returns>Array of Fieldinfo</returns>
        public static FieldInfo[] GetFields(this object target, BindingFlags flags)
        {
            System.Type type = target.GetType();
            return type.GetFields(flags);
        }
        /// <summary>
        /// Get all Property from object by filtering bindingflags
        /// </summary>
        /// <param name="target">the object refrence</param>
        /// <param name="flags">make filtering witch type of Property</param>
        /// <returns>Array of PropertyInfo</returns>
        public static PropertyInfo[] GetProperties(this object target, BindingFlags flags)
        {
            System.Type type = target.GetType();
            return type.GetProperties(flags);
        }
        public static IIdentifier GetID(this object obj)
        {
            return GetID<IIdentifier>(obj);
        }
        public static T GetID<T>(this object obj) where T : IIdentifier
        {
            if (obj is T Provider)
            {
                return Provider;
            }
            else
            {
                return default;
            }
        }
        public static bool HasNameID(this object obj, string NameID)
        {
            if (obj is INameIdentifier Provider)
            {
                return Provider.HasNameID(NameID);
            }
            else
            {
                Debug.LogError($"Target Object({obj}) should implement {typeof(INameIdentifier)} interface.");
                return false;
            }
        }
        public static void Destroy(this System.IDisposable provider)
        {

        }
    }
}