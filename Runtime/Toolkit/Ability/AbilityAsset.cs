using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class AbilityAsset : TemplateAsset, IAbility, ICooldown
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
                IAbilityEffect[] Effects = GetEffects();
                if (Effects != null)
                {
                    foreach (var effect in Effects)
                    {
                        effect.Apply(user, context);
                    }
                    ((ICooldown)this).StartCooldown();
                    return true;
                }
                else
                {
                    Debug.LogWarning($"There isn't any effect in {this}");
                    return false;
                }
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

#if UNITY_EDITOR
        public override void OnEditorPlay()
        {
            lastUsedTime = -Mathf.Infinity;
        }
#endif
    }
    public abstract class AbilityEffectAsset : AbilityAsset
    {
        [ReadOnly]
        public string[] effects;

        private List<object> effectsObject = new List<object>();
        private IAbilityEffect[] myEffects;

        // AbilityAsset Method
        protected sealed override IAbilityEffect[] GetEffects()
        {
            //return null;
            if (myEffects == null)
            {
                if (effects != null)
                {
                    effectsObject.Clear();
                    List<IAbilityEffect> result = new List<IAbilityEffect>();
                    for (int i = 0; i < effects.Length; i++)
                    {
                        object targetObj = System.Activator.CreateInstance(GetClassType(effects[i]));
                        if (targetObj is IAbilityEffect provider)
                        {
                            provider.Initiate(this);
                            effectsObject.Add(targetObj);
                            result.Add(provider);
                        }
                        else
                        {
                            Debug.LogWarning($"The target object {targetObj} not implement {typeof(IAbilityEffect)}");
                        }

                    }
                    myEffects = result.ToArray();
                }
                else
                {
                    return null;
                }
            }
            return myEffects;
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

#if UNITY_EDITOR
        public override void OnEditorPlay()
        {
            base.OnEditorPlay();
            myEffects = null;
        }
#endif
    }
}