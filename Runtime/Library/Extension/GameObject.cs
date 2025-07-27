using System;
using UnityEngine;
using System.Reflection;

namespace RealMethod
{
    public static class GameObject_Extension
    {
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