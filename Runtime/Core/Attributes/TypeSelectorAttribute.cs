
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Linq;



namespace RealMethod
{
    public sealed class TypeSelectorAttribute : PropertyAttribute
    {
        public Type BaseType { get; private set; }

        public TypeSelectorAttribute(Type baseType = null)
        {
            BaseType = baseType ?? typeof(object);
        }
    }
}