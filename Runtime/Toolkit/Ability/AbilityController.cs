using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    public interface IAbilityAction : IAbility
    {
        void Initializer(AbilityController controller);
        void UseInput(InputAction.CallbackContext context);
    }

    public abstract class AbilityController : MonoBehaviour
    {
        [System.Serializable]
        private class InputAbility : SerializableDictionary<InputActionReference, AbilityAction> { }
        [Header("Setting")]
        [SerializeField]
        private InputAbility abilities;

        private void Awake()
        {
            foreach (var abil in abilities)
            {
                ((IAbilityAction)abil.Key).Initializer(this);
            }
        }
        private void OnEnable()
        {
            foreach (var abil in abilities)
            {
                abil.Key.action.performed += ((IAbilityAction)abil.Value).UseInput;
            }
        }
        private void OnDisable()
        {
            foreach (var abil in abilities)
            {
                abil.Key.action.performed -= ((IAbilityAction)abil.Value).UseInput;
            }
        }

        // Abstract Methods
        public abstract IAbilityContext GetTarget(AbilityAction ability);
    }




}