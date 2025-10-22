using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    public interface IAbilityAction : IAbility
    {
        void Initializer(AbilityController controller);
        void UseInput(InputAction.CallbackContext context);
        void OnEnableInput();
        void OnDisableInput();
    }
    public abstract class AbilityAction : AbilityEffectAsset, IAbilityAction
    {
        protected AbilityController MyController { get; private set; }

        // Implement IAbilityInitializer Interface
        void IAbilityAction.Initializer(AbilityController controller)
        {
            MyController = controller;
            OnInitiate();
        }
        void IAbilityAction.UseInput(InputAction.CallbackContext context)
        {
            if (CheckInput(context))
            {
                TryUse(MyController.gameObject, MyController.GetTarget(this));
            }
        }

        // Abstract Methods
        protected abstract void OnInitiate();
        public abstract void OnEnableInput();
        public abstract void OnDisableInput();
        protected abstract bool CheckInput(InputAction.CallbackContext context);
    }


    public abstract class AbilityController : MonoBehaviour
    {
        [System.Serializable]
        private class InputAbility : SerializableDictionary<InputActionReference, AbilityAsset> { }

        [Header("Setting")]
        [SerializeField]
        private bool bindInputs = false;
        [SerializeField]
        private InputAbility abilites;

        public IAbility[] myActions;
        public int Count => myActions != null ? myActions.Length : 0;

        // Unity Methods
        private void Awake()
        {
            myActions = new IAbility[abilites.Count];
            for (int i = 0; i < myActions.Length; i++)
            {
                IAbility action = Instantiate(abilites.GetValue(i));
                if (action is IAbilityAction provider)
                {
                    provider.Initializer(this);
                }
                myActions[i] = action;
            }
        }
        private void OnEnable()
        {
            if (bindInputs)
            {
                for (int i = 0; i < myActions.Length; i++)
                {
                    var action = myActions[i];
                    if (action is IAbilityAction provider)
                    {
                        abilites.GetKey(i).action.performed += provider.UseInput;
                        provider.OnEnableInput();
                    }
                }
            }
        }
        private void OnDisable()
        {
            if (bindInputs)
            {
                for (int i = 0; i < myActions.Length; i++)
                {
                    var action = myActions[i];
                    if (action is IAbilityAction provider)
                    {
                        abilites.GetKey(i).action.performed -= provider.UseInput;
                        provider.OnDisableInput();
                    }
                }
            }
        }
        private void OnDestroy()
        {
            myActions = null;
        }

        // Public Functions
        public IAbility GetAbility(int index)
        {
            return myActions[index];
        }


        // Abstract Methods
        public abstract IAbilityContext GetTarget(AbilityAsset ability);
    }

}