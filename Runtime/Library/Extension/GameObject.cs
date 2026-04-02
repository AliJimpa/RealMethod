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
        /// <summary>
        /// Attaches this GameObject to a specified parent GameObject, optionally preserving world position.
        /// Also sends an "OnAttach" message to the owner GameObject.
        /// </summary>
        /// <param name="owner">The GameObject to attach.</param>
        /// <param name="parent">The GameObject to attach to.</param>
        /// <param name="worldPositionStays">If true, the parent-relative position, rotation, and scale are modified so that the last local transformation is equal to the world transformation.</param>
        public static void Attach(this GameObject owner, GameObject parent, bool worldPositionStays)
        {
            owner.transform.SetParent(parent.transform, worldPositionStays);
            owner.SendAttachEvent(parent);
        }
        /// <summary>
        /// Detaches this GameObject from its current parent and sends an "OnDetach" message.
        /// </summary>
        /// <param name="owner">The GameObject to detach.</param>
        /// <param name="parent">The parent GameObject it was attached to (primarily for context, not used in the current implementation).</param>
        public static void Detach(this GameObject owner, GameObject parent)
        {
            owner.transform.SetParent(null);
            owner.SendDetachEvent();
        }
        /// <summary>
        /// Sends an "OnAttach" message to the owner GameObject.
        /// </summary>
        /// <param name="owner">The GameObject to send the message to.</param>
        /// <param name="spawner">The object that triggered the attach event (passed as parameter to the message).</param>
        /// <param name="option">Specifies whether the message must be received, or if it is optional.</param>
        public static void SendAttachEvent(this GameObject owner, Object spawner, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            owner.SendMessage("OnAttach", spawner, option);
        }
        /// <summary>
        /// Sends an "OnDetach" message to the owner GameObject.
        /// </summary>
        /// <param name="owner">The GameObject to send the message to.</param>
        /// <param name="option">Specifies whether the message must be received, or if it is optional.</param>
        public static void SendDetachEvent(this GameObject owner, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            owner.SendMessage("OnDetach", option);
        }
        /// <summary>
        /// Sends an "OnSpawn" message to the owner GameObject.
        /// </summary>
        /// <param name="owner">The GameObject to send the message to.</param>
        /// <param name="spawner">The object that triggered the spawn event (passed as parameter to the message).</param>
        /// <param name="option">Specifies whether the message must be received, or if it is optional.</param>
        public static void SendSpawnEvent(this GameObject owner, Object spawner, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            owner.SendMessage("OnSpawn", spawner, option);
        }
        /// <summary>
        /// Sends an "OnDespawn" message to the owner GameObject.
        /// </summary>
        /// <param name="owner">The GameObject to send the message to.</param>
        /// <param name="despawner">The object that triggered the despawn event (passed as parameter to the message).</param>
        /// <param name="option">Specifies whether the message must be received, or if it is optional.</param>
        public static void SendDespawnEvent(this GameObject owner, Object despawner, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            owner.SendMessage("OnDespawn", despawner, option);
        }
        /// <summary>
        /// Adds a new component of type TComponent to the GameObject and initializes it without arguments.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component to add, must inherit from MonoBehaviour and IInitializable.</typeparam>
        /// <typeparam name="TArgument">This type parameter is unused in this overload.</typeparam>
        /// <param name="gameObject">The GameObject to add the component to.</param>
        /// <returns>The newly added and initialized component.</returns>
        public static TComponent AddComponent<TComponent, TArgument>(this GameObject gameObject)
        where TComponent : MonoBehaviour, IInitializable
        {
            var component = gameObject.AddComponent<TComponent>();
            component.Initialize();
            return component;
        }
        /// <summary>
        /// Adds a new component of type TComponent to the GameObject and initializes it with a single argument.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component to add, must inherit from MonoBehaviour and IInitializableWithArgument.</typeparam>
        /// <typeparam name="TArgument">The type of the argument used for initialization.</typeparam>
        /// <param name="gameObject">The GameObject to add the component to.</param>
        /// <param name="argument">The argument to pass to the component's Initialize method.</param>
        /// <returns>The newly added and initialized component.</returns>
        public static TComponent AddComponent<TComponent, TArgument>(this GameObject gameObject, TArgument argument)
        where TComponent : MonoBehaviour, IInitializableWithArgument<TArgument>
        {
            var component = gameObject.AddComponent<TComponent>();
            component.Initialize(argument);
            return component;
        }
        /// <summary>
        /// Adds a new component of type TComponent to the GameObject and initializes it with two arguments.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component to add, must inherit from MonoBehaviour and IInitializableWithTwoArgument.</typeparam>
        /// <typeparam name="TArgumentA">The type of the first argument used for initialization.</typeparam>
        /// <typeparam name="TArgumentB">The type of the second argument used for initialization.</typeparam>
        /// <param name="gameObject">The GameObject to add the component to.</param>
        /// <param name="argumentA">The first argument to pass to the component's Initialize method.</param>
        /// <param name="argumentB">The second argument to pass to the component's Initialize method.</param>
        /// <returns>The newly added and initialized component.</returns>
        public static TComponent AddComponent<TComponent, TArgumentA, TArgumentB>(this GameObject gameObject, TArgumentA argumentA, TArgumentB argumentB)
        where TComponent : MonoBehaviour, IInitializableWithTwoArgument<TArgumentA, TArgumentB>
        {
            var component = gameObject.AddComponent<TComponent>();
            component.Initialize(argumentA, argumentB);
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
        /// Registers the GameObject with a shared object system.
        /// </summary>
        /// <param name="obj">The GameObject to register.</param>
        public static void Share(this GameObject obj)
        {
            Game.Bridge.AddSharedObject(obj);
        }
        /// <summary>
        /// Unregisters the GameObject from the shared object system.
        /// </summary>
        /// <param name="obj">The GameObject to unregister.</param>
        public static void Unshare(this GameObject obj)
        {
            Game.Bridge.RemoveSharedObject(obj);
        }
        /// <summary>
        /// Checks if the GameObject is currently registered in the shared object system.
        /// </summary>
        /// <param name="obj">The GameObject to check.</param>
        /// <returns>True if the GameObject is shared, false otherwise.</returns>
        public static bool IsShared(this GameObject obj)
        {
            return Game.Bridge.IsSharedObject(obj);
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