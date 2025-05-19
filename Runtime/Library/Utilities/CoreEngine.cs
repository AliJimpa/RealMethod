using System;
using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    static class CoreEngine
    {
        public static Vector3 getRelativePosition(Transform origin, Vector3 position)
        {
            Vector3 distance = position - origin.position;
            Vector3 relativePosition = Vector3.zero;
            relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
            relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
            relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);
            return relativePosition;
        }

        public static void SetTransform(this Transform trans, Transform newtransform)
        {
            trans.position = newtransform.position;
            trans.rotation = newtransform.rotation;
            trans.localScale = newtransform.localScale;
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

        public static Transform GetSocket(this Transform trans, string socketname)
        {
            foreach (var item in trans.GetComponentsInChildren<Transform>())
            {
                if (item.gameObject.name == socketname)
                {
                    return item;
                }
            }
            Debug.LogError($"Not find any socket with {socketname} name");
            return trans;
        }
    }
}