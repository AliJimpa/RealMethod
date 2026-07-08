using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    public sealed class DependencyInjection : GameSubsystem
    {
        private readonly object _lock = new();
        private Dictionary<Type, WeakReference<object>> bindings => Data.Repository;


        // GameSubsystem Methods
        protected override void OnBegin()
        {
        }
        protected override void OnEnd()
        {
        }



        /// <summary>
        /// Registers the specified instance as a dependency in the DI container.
        /// After registration, any object resolving or being injected 
        /// with the same type <typeparamref name="T"/> will receive this instance.
        /// </summary>
        public void Register<T>(T instance, bool overwrite = false)
        {
            lock (_lock)
            {
                Type objectType = typeof(T);
                if (bindings.TryGetValue(objectType, out var weak))
                {
                    if (!weak.TryGetTarget(out _))
                    {
                        bindings[objectType] = new WeakReference<object>(instance);
                        return;
                    }

                    if (overwrite)
                    {
                        bindings[objectType] = new WeakReference<object>(instance);
                        return;
                    }

                    throw new InvalidOperationException(
                        $"Instance of type {objectType.Name} is already registered and still alive.");
                }


                bindings[objectType] = new WeakReference<object>(instance);
            }
        }
        public bool Unregister<T>()
        {
            var type = typeof(T);
            lock (_lock)
            {
                return Unregister(type);
            }
        }
        public bool Unregister(Type type)
        {
            lock (_lock)
            {
                if (bindings.ContainsKey(type))
                {
                    return bindings.Remove(type);
                }
                return false;
            }
        }


        /// <summary>
        /// Injects all fields, properties, and methods marked with [Inject].
        /// Works for MonoBehaviours, ScriptableObjects, and normal classes.
        /// </summary>
        public void Inject(object target)
        {
            var type = target.GetType();

            // Fields
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (Attribute.IsDefined(field, typeof(InjectAttribute)))
                {
                    var dependency = Resolve(field.FieldType);
                    field.SetValue(target, dependency);
                }
            }

            // Properties
            foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (Attribute.IsDefined(prop, typeof(InjectAttribute)) && prop.CanWrite)
                {
                    var dependency = Resolve(prop.PropertyType);
                    prop.SetValue(target, dependency);
                }
            }

            // Methods
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (Attribute.IsDefined(method, typeof(InjectAttribute)))
                {
                    var parameters = method.GetParameters();
                    var args = parameters.Select(p => Resolve(p.ParameterType)).ToArray();
                    method.Invoke(target, args);
                }
            }
        }


        private object Resolve(Type type)
        {
            // If already registered, return directly
            if (bindings != null)
            {
                if (bindings.TryGetValue(type, out var weak))
                {
                    if (weak.TryGetTarget(out object existing))
                        return existing;
                }

            }

            Debug.LogError($"Cannot resolve type {type.Name}: no registered instance or injectable constructor.");
            return null;
        }

#if UNITY_EDITOR
        protected override string GetInspectorInfor()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < bindings.Count; i++)
            {
                var type = bindings.GetKey(i);
                if (typeof(IFeature).IsAssignableFrom(type))
                {
                    var weakRef = bindings.GetValue(i);
                    if (weakRef.TryGetTarget(out var target))
                        sb.AppendLine($"{i}. {type.Name} -> Alive ({target})");
                    else
                        sb.AppendLine($"{i}. {type.Name} -> Collected");
                }
            }
            return sb.ToString();
        }
#endif
    }
}