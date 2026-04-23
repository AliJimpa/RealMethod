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
        public static void InvokeSaveEvent(this object obj, SendMessageOptions option = SendMessageOptions.RequireReceiver)
        {
            if (obj is ISave provider)
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
            if (obj is ISave provider)
            {
                provider.OnLoaded();
            }
            else
            {
                obj.SendMessage(MessageNames.Load, option);
            }
        }
        public static void SendMessage(this object so, string methodName, SendMessageOptions option)
        {
            so.SendMessage(methodName, null, option);
        }
        public static void SendMessage(this object obj, string methodName, object parameter, SendMessageOptions options)
        {
            var method = obj.GetType().GetMethod(methodName,
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
    }
}