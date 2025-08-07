using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    public interface IAbilityInitializer
    {
        void Initializer(MonoBehaviour owner);
    }
    public abstract class AbilityAction : AbilityAsset, IAbilityInitializer
    {
        private MonoBehaviour MyOwner;
        private IAbilityContext TargetContext;

        void IAbilityInitializer.Initializer(MonoBehaviour owner)
        {
            MyOwner = owner;
            OnInitiate();
        }

        // Public Functions
        public void UseInput(InputAction.CallbackContext context)
        {
            TryUse(MyOwner.gameObject, TargetContext);
        }

        // Abstract Methods
        protected abstract void OnInitiate();
    }

}