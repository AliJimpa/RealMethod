using System;
using UnityEngine;
using System.Reflection;

namespace RealMethod
{
    public static class GameObject_Extension
    {
        public static TComponent AddComponent<TComponent, TArgument>(this GameObject gameObject)
        where TComponent : MonoBehaviour, IInitializable
        {
            var component = gameObject.AddComponent<TComponent>();
            component.Initialize();
            return component;
        }
        public static TComponent AddComponent<TComponent, TArgument>(this GameObject gameObject, TArgument argument)
        where TComponent : MonoBehaviour, IInitializableWithArgument<TArgument>
        {
            var component = gameObject.AddComponent<TComponent>();
            component.Initialize(argument);
            return component;
        }
        public static TComponent AddComponent<TComponent, TArgumentA, TArgumentB>(this GameObject gameObject, TArgumentA argumentA, TArgumentB argumentB)
        where TComponent : MonoBehaviour, IInitializableWithTwoArgument<TArgumentA, TArgumentB>
        {
            var component = gameObject.AddComponent<TComponent>();
            component.Initialize(argumentA, argumentB);
            return component;
        }
        public static TComponent CopyComponent<TComponent>(this GameObject desitation, TComponent originalComponent) where TComponent : Component
        {
            Type componentType = originalComponent.GetType();
            Component copy = desitation.AddComponent(componentType);
            FieldInfo[] fields = componentType.GetFields();
            foreach (FieldInfo field in fields)
            {
                field.SetValue(copy, field.GetValue(originalComponent));
            }
            return copy as TComponent;
        }
    }
}