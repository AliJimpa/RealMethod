using UnityEngine;

namespace RealMethod
{
    public interface IInitializableWithArgument<TArgument>
    {
        void Initialize(TArgument argument);
    }

    static class Initializable
    {
        public static TComponent AddComponent<TComponent, TArgument>(this GameObject gameObject, TArgument argument)
        where TComponent : MonoBehaviour, IInitializableWithArgument<TArgument>
        {
            var component = gameObject.AddComponent<TComponent>();
            component.Initialize(argument);
            return component;
        }
    }



}


