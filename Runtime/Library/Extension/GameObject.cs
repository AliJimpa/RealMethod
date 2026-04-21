using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RealMethod
{
    /// <summary>
    /// Extension methods for the GameObject class to add common functionalities.
    /// </summary>
    public static class GameObject_Extension
    {
        public static void Death(this GameObject target)
        {
            IDamageable provider = target.GetComponent<IDamageable>();
            if (provider != null)
            {
                provider.Die();
            }
            else
            {
                target.SendMessage(MessageNames.Die, SendMessageOptions.RequireReceiver);
            }
        }
        /// <summary>
        /// pplies damage to the target GameObject.
        /// </summary>
        /// <param name="target">The GameObject that should receive damage.</param>
        /// <param name="attacker">ther GameObject how applyed damage</param>
        /// <param name="damage">The damage payload containing specific value</param>
        public static void ApplyDamage(this GameObject target, GameObject attacker, float damage)
        {
            target.ApplyDamage(new HitData(new RaycastHit(), attacker, damage));
        }
        /// <summary>
        /// Applies damage to the target GameObject using two fallback methods:
        /// 1) If the object implements <see cref="IDamageable"/>, damage is applied directly through <c>TakeDamage</c>.
        /// 2) Otherwise, a Unity <c>SendMessage</c> call is made using <c>GameMessages.ApplyDamage</c>.
        /// 
        /// This allows all GameObjects to receive damage regardless of whether they use the interface 
        /// or the older SendMessage-based system.
        /// </summary>
        /// <param name="target">The GameObject that should receive damage.</param>
        /// <param name="hitdata">The damage payload containing values such as damage amount and hit details.</param>
        public static void ApplyDamage(this GameObject target, HitData hitdata)
        {
            IDamageable provider = target.GetComponent<IDamageable>();
            if (provider != null)
            {
                provider.TakeDamage(hitdata);
            }
            else
            {
                target.SendMessage(MessageNames.ApplyDamage, hitdata, SendMessageOptions.RequireReceiver);
            }
        }
        /// <summary>
        /// Attaches this GameObject to a specified Socket transform, optionally preserving world position.
        /// Also sends an "OnAttach" message to the owner GameObject.
        /// </summary>
        /// <param name="target">The GameObject to attach.</param>
        /// <param name="parent">The Transform to get Socket from that.</param>
        /// <param name="socketname">The Socket name to attach to</param>
        /// <param name="worldPositionStays">If true, the parent-relative position, rotation, and scale are modified so that the last local transformation is equal to the world transformation.</param>
        public static void Attach(this GameObject target, Transform parent, Name16 socketname, bool worldPositionStays = true)
        {
            Transform MySocket = parent.GetSocket(socketname);
            MySocket.SetParent(parent.transform, worldPositionStays);
            target.SendMessage(MessageNames.Attach, parent, SendMessageOptions.DontRequireReceiver);
        }
        /// <summary>
        /// Attaches this GameObject to a specified parent GameObject, optionally preserving world position.
        /// Also sends an "OnAttach" message to the owner GameObject.
        /// </summary>
        /// <param name="target">The GameObject to attach.</param>
        /// <param name="parent">The GameObject to attach to.</param>
        /// <param name="worldPositionStays">If true, the parent-relative position, rotation, and scale are modified so that the last local transformation is equal to the world transformation.</param>
        public static void Attach(this GameObject target, GameObject parent, bool worldPositionStays = true)
        {
            target.transform.SetParent(parent.transform, worldPositionStays);
            target.SendMessage(MessageNames.Attach, parent, SendMessageOptions.DontRequireReceiver);
        }
        /// <summary>
        /// Detaches this GameObject from its current parent and sends an "OnDetach" message.
        /// </summary>
        /// <param name="target">The GameObject to detach.</param>
        /// <param name="parent">The parent GameObject it was attached to (primarily for context, not used in the current implementation).</param>
        public static void Detach(this GameObject target, GameObject parent)
        {
            target.transform.SetParent(null);
            target.SendMessage(MessageNames.Detach, SendMessageOptions.DontRequireReceiver);
        }
        /// <summary>
        /// Sends an "OnSpawn" message to the owner GameObject.
        /// </summary>
        /// <param name="owner">The GameObject to send the message to.</param>
        /// <param name="spawner">The object that triggered the spawn event (passed as parameter to the message).</param>
        /// <param name="option">Specifies whether the message must be received, or if it is optional.</param>
        public static void InvokeSpawnEvent(this GameObject owner, Object spawner = null, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            if (spawner != null)
            {
                ISpawnWithAuthor provider = owner.GetComponent<ISpawnWithAuthor>();
                if (provider != null)
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
                ISpawn provider = owner.GetComponent<ISpawn>();
                if (provider != null)
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
        /// <param name="option">Specifies whether the message must be received, or if it is optional.</param>
        public static void InvokeDespawnEvent(this GameObject owner, Object despawner = null, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            if (despawner != null)
            {
                IDespawnWithAuthor provider = owner.GetComponent<IDespawnWithAuthor>();
                if (provider != null)
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
                IDespawn provider = owner.GetComponent<IDespawn>();
                if (provider != null)
                {
                    provider.OnDespawn();
                }
                else
                {
                    owner.SendMessage(MessageNames.Despawn, option);
                }
            }
        }
        /// <summary>
        /// Adds a new component of type TComponent to the GameObject and initializes it without arguments.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component to add, must inherit from MonoBehaviour and IInitializable.</typeparam>
        /// <typeparam name="TArgument">This type parameter is unused in this overload.</typeparam>
        /// <param name="target">The GameObject to add the component to.</param>
        /// <returns>The newly added and initialized component.</returns>
        public static TComponent AddComponent<TComponent, TArgument>(this GameObject target)
        where TComponent : MonoBehaviour, ISpawn
        {
            var component = target.AddComponent<TComponent>();
            component.OnSpawn();
            return component;
        }
        /// <summary>
        /// Adds a new component of type TComponent to the GameObject and initializes it with a single argument.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component to add, must inherit from MonoBehaviour and IInitializableWithArgument.</typeparam>
        /// <typeparam name="TArgument">The type of the argument used for initialization.</typeparam>
        /// <param name="target">The GameObject to add the component to.</param>
        /// <param name="argument">The argument to pass to the component's Initialize method.</param>
        /// <returns>The newly added and initialized component.</returns>
        public static TComponent AddComponent<TComponent, TArgument>(this GameObject target, TArgument argument)
        where TComponent : MonoBehaviour, ISpawnWithArgument<TArgument>
        {
            var component = target.AddComponent<TComponent>();
            component.OnSpawn(argument);
            return component;
        }
        /// <summary>
        /// Adds a new component of type TComponent to the GameObject and initializes it with two arguments.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component to add, must inherit from MonoBehaviour and IInitializableWithTwoArgument.</typeparam>
        /// <typeparam name="TArgumentA">The type of the first argument used for initialization.</typeparam>
        /// <typeparam name="TArgumentB">The type of the second argument used for initialization.</typeparam>
        /// <param name="target">The GameObject to add the component to.</param>
        /// <param name="argumentA">The first argument to pass to the component's Initialize method.</param>
        /// <param name="argumentB">The second argument to pass to the component's Initialize method.</param>
        /// <returns>The newly added and initialized component.</returns>
        public static TComponent AddComponent<TComponent, TArgumentA, TArgumentB>(this GameObject target, TArgumentA argumentA, TArgumentB argumentB)
        where TComponent : MonoBehaviour, ISpawnWithTwoArgument<TArgumentA, TArgumentB>
        {
            var component = target.AddComponent<TComponent>();
            component.OnSpawn(argumentA, argumentB);
            return component;
        }
        /// <summary>
        /// Copies all public fields from an original component to a new component of the same type on a destination GameObject.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component to copy. Must inherit from Component.</typeparam>
        /// <param name="desitation">The GameObject to add the copied component to.</param>
        /// <param name="originalComponent">The component to copy from.</param>
        /// <returns>The newly created and populated component on the destination GameObject.</returns>
        public static TComponent CopyComponent<TComponent>(this GameObject desitation, TComponent originalComponent) where TComponent : Component
        {
            System.Type componentType = originalComponent.GetType();
            Component copy = desitation.AddComponent(componentType);
            FieldInfo[] fields = componentType.GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(originalComponent));
            }
            return copy as TComponent;
        }
        /// <summary>
        /// Checks if a GameObject is a valid scene object (not a prefab asset, and belongs to a scene).
        /// This method's behavior differs slightly between Editor and runtime.
        /// </summary>
        /// <param name="obj">The GameObject to check.</param>
        /// <returns>True if the GameObject is in a scene, false otherwise.</returns>
        public static bool IsInScene(this GameObject obj)
        {
#if UNITY_EDITOR
            return PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.NotAPrefab
                    && obj.scene.IsValid();
#else
            return obj.scene.IsValid() && obj.scene.name != null;
#endif
        }
        /// <summary>
        /// Check if GameObject was lives in DontDestroyOnload scene
        /// </summary>
        /// <param name="obj">The GameObject to check.</param>
        /// <returns>True if the GameObject is in DontDestroyOnload scene, false otherwise.</returns>
        public static bool IsDontDestroyOnLoad(this GameObject obj)
        {
            return obj.scene.name == "DontDestroyOnLoad";
        }
        /// <summary>
        /// Set layer on object and children
        /// </summary>
        /// <param name="target">GameObject Target</param>
        /// <param name="layer">the layer you want to set that</param>
        public static void SetLayerRecursively(this GameObject target, int layer)
        {
            foreach (Transform t in target.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = layer;
            }
        }
        /// <summary>
        /// Checks whether the GameObject contains a component that implements or matches type T.
        /// </summary>
        /// <typeparam name="T">The interface or component type to search for.</typeparam>
        /// <param name="target">The GameObject to check.</param>
        /// <returns>True if a component of type T exists on the GameObject; otherwise false.</returns>
        public static bool HasImplementInterface<T>(this GameObject target)
        {
            if (target != null)
            {
                T provider = target.GetComponent<T>();
                if (provider != null)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Checks whether the GameObject contains a component that implements or matches type T
        /// and returns the found component.
        /// </summary>
        /// <typeparam name="T">The interface or component type to search for.</typeparam>
        /// <param name="target">The GameObject to check.</param>
        /// <param name="Result">Outputs the found component if one exists.</param>
        /// <returns>True if a component of type T exists on the GameObject; otherwise false.</returns>
        public static bool HasImplementInterface<T>(this GameObject target, out T Result)
        {
            if (target != null)
            {
                T provider = target.GetComponent<T>();
                if (provider != null)
                {
                    Result = provider;
                    return true;
                }
            }

            Result = default;
            return false;

        }


#if UNITY_EDITOR
        /// <summary>
        /// Checks if the GameObject is a prefab asset itself (not an instance of a prefab).
        /// </summary>
        /// <param name="obj">The GameObject to check.</param>
        /// <returns>True if it's a prefab asset, false otherwise.</returns>
        public static bool IsPrefabAsset(this GameObject obj)
        {
            return PrefabUtility.GetPrefabAssetType(obj) != PrefabAssetType.NotAPrefab
                   && PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.NotAPrefab;
        }
        /// <summary>
        /// Checks if the GameObject is an instance of a connected prefab.
        /// </summary>
        /// <param name="obj">The GameObject to check.</param>
        /// <returns>True if it's a connected prefab instance, false otherwise.</returns>
        public static bool IsPrefabInstance(this GameObject obj)
        {
            return PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.Connected;
        }
#endif

    }
}