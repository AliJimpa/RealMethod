using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.RepresentationModel;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RealMethod
{
    public abstract class AbilityAsset : DataAsset, IAbility, ICooldown
    {
        private float lastUsedTime = -Mathf.Infinity;

        // Implement IIdentifier Interface
        string IIdentifier.NameID => name;
        // Implement IAbility Interface
        public bool CanUse(GameObject user)
        {
            if (user == null)
            {
                Debug.LogWarning($"{this} can't use user is not valid");
                return false;
            }
            return Prerequisite(user) && ((ICooldown)this).IsAvailable;
        }
        public bool TryUse(GameObject user, IAbilityContext context)
        {
            if (CanUse(user))
            {
                foreach (var effect in GetEffects())
                {
                    effect.Apply(user, context);
                }
                ((ICooldown)this).StartCooldown();
                return true;
            }
            return false;
        }
        // Implement ICooldown Interface
        public float CooldownDuration => GetCooldown();
        bool ICooldown.IsAvailable => Time.time > lastUsedTime + GetCooldown();
        void ICooldown.StartCooldown()
        {
            lastUsedTime = Time.time;
        }

        // Public Functions
        public void ResetCooldown()
        {
            lastUsedTime = -Mathf.Infinity;
        }

        // Abstract Methods
        protected abstract float GetCooldown();
        protected abstract IAbilityEffect[] GetEffects();
        protected abstract bool Prerequisite(GameObject user);
    }
    public abstract class AbilityEffectAsset : AbilityAsset
    {
        [Header("Effect")]
        public string[] effects;
        public object[] cache;

        // AbilityAsset Method
        protected sealed override IAbilityEffect[] GetEffects()
        {
            cache = new Object[effects.Length];
            for (int i = 0; i < effects.Length; i++)
            {
                cache[i] = System.Activator.CreateInstance(GetClassType(effects[i]));
            }
            List<IAbilityEffect> result = new List<IAbilityEffect>();
            foreach (var ca in cache)
            {
                if (ca is IAbilityEffect provider)
                {
                    result.Add(provider);
                }
                else
                {
                    Debug.LogWarning($"The target object {ca} not implement {typeof(IAbilityEffect)}");
                }
            }
            return result.ToArray();
        }


        private System.Type GetClassType(string classname)
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(classname);
                if (type != null)
                    return type;
            }
            return null;
        }
    }

    public abstract class AbilityAction : AbilityEffectAsset, IAbilityAction
    {
        private AbilityController MyController;

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
        protected abstract bool CheckInput(InputAction.CallbackContext context);
    }


}