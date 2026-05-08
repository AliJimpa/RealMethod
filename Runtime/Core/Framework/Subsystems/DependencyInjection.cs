using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RealMethod
{
    public interface IFeature
    {

    }


    public sealed class DependencyInjection : GameSubsystem
    {
        private readonly object _lock = new();
        private Dictionary<Type, WeakReference<object>> bindings => Data.Repository;

        public DependencyInjection(ShareData data) : base(data)
        {
        }


        /// <summary>
        /// Registers the specified instance as a dependency in the DI container.
        /// After registration, any object resolving or being injected 
        /// with the same type <typeparamref name="T"/> will receive this instance.
        /// </summary>
        public void Bind<TInterface>(TInterface instance)
        {
            lock (_lock)
            {
                bindings[typeof(TInterface)] = new WeakReference<object>(instance);
            }
        }

        /// <summary>
        /// Resolves a dependency of the specified type.
        /// Automatically constructs non-MonoBehaviour classes using constructor injection.
        /// </summary>
        public static T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public static object Resolve(Type type)
        {
            // If already registered, return directly
            // if (bindings.TryGetValue(type, out var existing))
            //     return existing;

            // Handle constructor injection
            var ctor = GetInjectableConstructor(type);
            if (ctor != null)
            {
                var parameters = ctor.GetParameters();
                var args = parameters.Select(p => Resolve(p.ParameterType)).ToArray();
                var instance = Activator.CreateInstance(type, args);

                // Optionally inject remaining [Inject] fields
                Inject(instance);

                return instance;
            }

            // Fallback for parameterless types
            var parameterless = type.GetConstructor(Type.EmptyTypes);
            if (parameterless != null)
            {
                var instance = Activator.CreateInstance(type);
                Inject(instance);
                return instance;
            }

            throw new InvalidOperationException($"Cannot resolve type {type.Name}: no registered instance or injectable constructor.");
        }

        private static ConstructorInfo GetInjectableConstructor(Type type)
        {
            // Check if any constructor has [Inject]
            var markedCtor = type.GetConstructors()
                .FirstOrDefault(c => Attribute.IsDefined(c, typeof(InjectAttribute)));
            if (markedCtor != null)
                return markedCtor;

            // Otherwise, pick the one with most parameters for best DI heuristics
            return type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();
        }

        /// <summary>
        /// Injects all fields, properties, and methods marked with [Inject].
        /// Works for MonoBehaviours, ScriptableObjects, and normal classes.
        /// </summary>
        public static void Inject(object target)
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




        public TInterface Resolve2<TInterface>()
        {
            lock (_lock)
            {
                Type type = typeof(TInterface);
                if (bindings.ContainsKey(type))
                {
                    if (bindings[type].TryGetTarget(out object target))
                    {
                        return (TInterface)target;
                    }
                    bindings.Remove(type);
                }
                return default;
            }
        }

#if UNITY_EDITOR
        protected override string GetInspectorInfor()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < bindings.Count; i++)
            {
                var type = bindings.GetKey(i);
                if (type is IFeature)
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